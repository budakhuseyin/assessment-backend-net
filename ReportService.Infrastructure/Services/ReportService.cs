using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using ReportService.Application.DTOs;
using ReportService.Application.Interfaces.Repositories;
using ReportService.Application.Interfaces.Services;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using Shared.Messages.Events;
using System.Text.Json;

namespace ReportService.Infrastructure.Services;

/// <summary>
/// Raporlama iş kurallarını uygulayan servis sınıfı.
/// Rapor talebi alındığında "Preparing" statüsünde kaydeder ve
/// RabbitMQ kuyruğuna ReportRequestedEvent yayınlar.
/// Cache-Aside pattern kullanılarak rapor listeleri Redis'te önbelleğe alınır.
/// </summary>
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IDistributedCache _cache;

    // Cache anahtar sabitleri
    private const string AllReportsCacheKey = "all_reports";
    private static string ReportByIdCacheKey(Guid id) => $"report_{id}";

    // Cache süresi: 2 dakika (raporlar dinamik olduğu için daha kısa)
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
    };

    public ReportService(IReportRepository reportRepository, IPublishEndpoint publishEndpoint, IDistributedCache cache)
    {
        _reportRepository = reportRepository;
        _publishEndpoint = publishEndpoint;
        _cache = cache;
    }

    public async Task<ReportResponse> CreateReportAsync()
    {
        var report = new Report
        {
            UUID = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            Status = ReportStatus.Preparing
        };

        await _reportRepository.AddAsync(report);

        // Cache Invalidation: Yeni rapor oluşunca liste cache'ini geçersiz kıl
        await _cache.RemoveAsync(AllReportsCacheKey);

        // ContactService bu eventi dinleyip istatistikleri hesaplayacak ve raporu güncelleyecek
        await _publishEndpoint.Publish(new ReportRequestedEvent { ReportId = report.UUID });

        return MapToResponse(report);
    }

    public async Task<IEnumerable<ReportResponse>> GetAllAsync()
    {
        // 1. Önce Redis'e bak
        var cached = await _cache.GetStringAsync(AllReportsCacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<IEnumerable<ReportResponse>>(cached)!;

        // 2. Redis'te yoksa veritabanına git
        var reports = await _reportRepository.GetAllWithDetailsAsync();
        var response = reports.Select(MapToResponse).ToList();

        // 3. Sonucu Redis'e yaz
        await _cache.SetStringAsync(AllReportsCacheKey,
            JsonSerializer.Serialize(response), CacheOptions);

        return response;
    }

    public async Task<ReportResponse?> GetByIdAsync(Guid id)
    {
        var cacheKey = ReportByIdCacheKey(id);

        // 1. Önce Redis'e bak
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<ReportResponse>(cached);

        // 2. Redis'te yoksa veritabanına git
        var report = await _reportRepository.GetByIdWithDetailsAsync(id);
        if (report == null) return null;

        var response = MapToResponse(report);

        // 3. Sonucu Redis'e yaz
        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(response), CacheOptions);

        return response;
    }

    public async Task<bool> CompleteReportAsync(Guid reportId, CompleteReportRequest request)
    {
        var newDetails = request.Details.Select(d => new ReportDetail
        {
            UUID = Guid.NewGuid(),
            Location = d.Location,
            PersonCount = d.PersonCount,
            PhoneNumberCount = d.PhoneNumberCount,
            ReportUUID = reportId
        }).ToList();

        var result = await _reportRepository.CompleteAsync(reportId, newDetails);

        if (result)
        {
            // Cache Invalidation: Rapor tamamlanınca hem liste hem tekil cache'i temizle
            await _cache.RemoveAsync(AllReportsCacheKey);
            await _cache.RemoveAsync(ReportByIdCacheKey(reportId));
        }

        return result;
    }

    // Entity'den DTO'ya dönüşüm (private yardımcı metod)
    private static ReportResponse MapToResponse(Report report) => new()
    {
        UUID = report.UUID,
        RequestedAt = report.RequestedAt,
        Status = report.Status,
        ReportDetails = report.ReportDetails.Select(d => new ReportDetailResponse
        {
            UUID = d.UUID,
            Location = d.Location,
            PersonCount = d.PersonCount,
            PhoneNumberCount = d.PhoneNumberCount
        }).ToList()
    };
}
