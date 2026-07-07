using ReportService.Application.DTOs;

namespace ReportService.Application.Interfaces.Services;

/// <summary>
/// Raporlama iş kurallarını tanımlayan servis arayüzü.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Yeni bir rapor talebi oluşturur (Status: Preparing) ve RabbitMQ kuyruğuna mesaj gönderir.
    /// </summary>
    Task<ReportResponse> CreateReportAsync();

    /// <summary>
    /// Tüm raporları listeler.
    /// </summary>
    Task<IEnumerable<ReportResponse>> GetAllAsync();

    /// <summary>
    /// UUID ile bir raporu detaylarıyla birlikte getirir.
    /// </summary>
    Task<ReportResponse?> GetByIdAsync(Guid id);
}
