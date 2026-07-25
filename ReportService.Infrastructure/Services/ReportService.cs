using MassTransit;
using ReportService.Application.DTOs;
using ReportService.Application.Interfaces.Repositories;
using ReportService.Application.Interfaces.Services;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using Shared.Messages.Events;

namespace ReportService.Infrastructure.Services;

/// <summary>
/// Raporlama iş kurallarını uygulayan servis sınıfı.
/// Rapor talebi alındığında "Preparing" statüsünde kaydeder ve
/// RabbitMQ kuyruğuna ReportRequestedEvent yayınlar.
/// </summary>
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ReportService(IReportRepository reportRepository, IPublishEndpoint publishEndpoint)
    {
        _reportRepository = reportRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ReportResponse> CreateReportAsync()
    {
        // Raporu "Hazırlanıyor" statüsünde oluştur ve veritabanına kaydet
        var report = new Report
        {
            UUID = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            Status = ReportStatus.Preparing
        };

        await _reportRepository.AddAsync(report);

        // RabbitMQ kuyruğuna mesaj bırak; ContactService bu mesajı dinleyip
        // istatistikleri hesaplayacak ve raporu güncelleyecek.
        await _publishEndpoint.Publish(new ReportRequestedEvent
        {
            ReportId = report.UUID
        });

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
