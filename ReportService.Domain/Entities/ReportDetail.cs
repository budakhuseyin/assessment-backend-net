namespace ReportService.Domain.Entities;

/// <summary>
/// Bir rapordaki lokasyon bazlı istatistik satırını temsil eden entity.
/// BaseEntity'den UUID ve CreatedAt alanlarını miras alır.
/// </summary>
public class ReportDetail : BaseEntity
{
    public string? Location { get; set; }
    public int PersonCount { get; set; }
    public int PhoneNumberCount { get; set; }

    // Foreign Key
    public Guid ReportUUID { get; set; }
    public Report? Report { get; set; }
}
