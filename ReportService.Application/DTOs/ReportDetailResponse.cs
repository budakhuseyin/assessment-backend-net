namespace ReportService.Application.DTOs;

/// <summary>
/// Rapor detay satırını (lokasyon bazlı istatistik) temsil eden yanıt modeli.
/// </summary>
public class ReportDetailResponse
{
    public Guid UUID { get; set; }
    public string? Location { get; set; }
    public int PersonCount { get; set; }
    public int PhoneNumberCount { get; set; }
}
