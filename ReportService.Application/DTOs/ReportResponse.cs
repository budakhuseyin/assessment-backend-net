using ReportService.Domain.Enums;

namespace ReportService.Application.DTOs;

/// <summary>
/// Rapor bilgilerini istemciye döndürmek için kullanılan yanıt modeli.
/// Record type: immutable, value-based equality.
/// </summary>
public record ReportResponse
{
    public Guid UUID { get; init; }
    public DateTime RequestedAt { get; init; }
    public ReportStatus Status { get; init; }
    public List<ReportDetailResponse> ReportDetails { get; init; } = new();
}
