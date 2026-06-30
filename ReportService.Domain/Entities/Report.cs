using ReportService.Domain.Enums;

namespace ReportService.Domain.Entities;

public class Report
{
    public Guid UUID { get; set; }
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public ICollection<ReportDetail> ReportDetails { get; set; }
    public Report()
    {
        ReportDetails = new List<ReportDetail>();
    }
}
