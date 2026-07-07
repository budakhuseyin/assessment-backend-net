using ReportService.Application.DTOs;
using ReportService.Application.Interfaces.Repositories;
using ReportService.Application.Interfaces.Services;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;

namespace ReportService.Infrastructure.Services;

/// <summary>
/// Raporlama iş kurallarını uygulayan servis sınıfı.
/// Rapor talebi alındığında veritabanına "Preparing" statüsünde kaydeder.
/// RabbitMQ entegrasyonu tamamlandığında bu servis kuyruğa mesaj gönderecek.
/// </summary>
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<ReportResponse> CreateReportAsync()
    {
        // Raporu "Hazırlanıyor" statüsünde oluştur
        var report = new Report
        {
            UUID = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            Status = ReportStatus.Preparing
        };

        await _reportRepository.AddAsync(report);

        // TODO: RabbitMQ entegrasyonu tamamlandığında buraya
        // IPublishEndpoint.Publish(new ReportRequestedEvent { ReportId = report.UUID })
        // kodu eklenecek.

        return MapToResponse(report);
    }

    public async Task<IEnumerable<ReportResponse>> GetAllAsync()
    {
        var reports = await _reportRepository.GetAllWithDetailsAsync();
        return reports.Select(MapToResponse);
    }

    public async Task<ReportResponse?> GetByIdAsync(Guid id)
    {
        var report = await _reportRepository.GetByIdWithDetailsAsync(id);
        return report == null ? null : MapToResponse(report);
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
