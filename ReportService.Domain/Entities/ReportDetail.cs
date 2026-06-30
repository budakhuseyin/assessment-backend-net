namespace ReportService.Domain.Entities;

public class ReportDetail
{
    public Guid UUID { get; set; }
    public string? Location { get; set; }
    public int PersonCount { get; set; }
    public int PhoneNumberCount { get; set; }
    // Foreign Key
    public Guid ReportUUID { get; set; }
    public Report? Report { get; set; }
}
