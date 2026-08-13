namespace ReportService.Application.DTOs;

/// <summary>
/// ContactService'ten gelen, raporu tamamlamak için kullanılan istek modeli.
/// Record type: immutable, value-based equality.
/// </summary>
public record CompleteReportRequest
{
    public List<ReportDetailRequest> Details { get; init; } = new();
}

public record ReportDetailRequest
{
    public string? Location { get; init; }
    public int PersonCount { get; init; }
    public int PhoneNumberCount { get; init; }
}
