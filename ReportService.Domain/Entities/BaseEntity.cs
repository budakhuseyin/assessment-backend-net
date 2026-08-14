namespace ReportService.Domain.Entities;

/// <summary>
/// Tüm domain entity'leri için ortak alanları ve davranışları tanımlayan temel sınıf.
/// Her entity UUID ve oluşturulma tarihi (CreatedAt) bilgisine sahip olur.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Kaydın benzersiz tanımlayıcısı (Primary Key).
    /// Yeni bir nesne oluşturulduğunda otomatik olarak üretilir.
    /// </summary>
    public Guid UUID { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Kaydın sisteme eklendiği tarih ve saat (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
