using ReportService.Domain.Enums;

namespace ReportService.Application.DTOs;

/// <summary>
/// Rapor bilgilerini istemciye döndürmek için kullanılan yanıt modeli.
/// </summary>
public class ReportResponse
{
    public Guid UUID { get; set; }
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public List<ReportDetailResponse> ReportDetails { get; set; } = new();
}
