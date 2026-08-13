namespace ReportService.Application.DTOs;

/// <summary>
/// Rapor detay satırını (lokasyon bazlı istatistik) temsil eden yanıt modeli.
/// Record type: immutable, value-based equality.
/// </summary>
public record ReportDetailResponse
{
    public Guid UUID { get; init; }
    public string? Location { get; init; }
    public int PersonCount { get; init; }
    public int PhoneNumberCount { get; init; }
}
