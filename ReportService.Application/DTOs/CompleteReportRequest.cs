namespace ReportService.Application.DTOs;

/// <summary>
/// ContactService'ten gelen, raporu tamamlamak için kullanılan istek modeli.
/// Her eleman bir lokasyon için hesaplanan istatistikleri temsil eder.
/// </summary>
public class CompleteReportRequest
{
    public List<ReportDetailRequest> Details { get; set; } = new();
}

public class ReportDetailRequest
{
    public string? Location { get; set; }
    public int PersonCount { get; set; }
    public int PhoneNumberCount { get; set; }
}
