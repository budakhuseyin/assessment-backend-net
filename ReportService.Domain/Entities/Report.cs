using ReportService.Domain.Enums;

namespace ReportService.Domain.Entities;

/// <summary>
/// Bir rapor talebini temsil eden entity.
/// BaseEntity'den UUID ve CreatedAt alanlarını miras alır.
/// RequestedAt, raporun istek zamanını özel olarak takip eder.
/// </summary>
public class Report : BaseEntity
{
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public ICollection<ReportDetail> ReportDetails { get; set; }

    public Report()
    {
        ReportDetails = new List<ReportDetail>();
    }
}
