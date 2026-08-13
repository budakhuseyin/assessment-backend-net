namespace ReportService.Application.DTOs;

/// <summary>
/// Rapor oluşturma talebini temsil eden istek modeli.
/// Record type: immutable, value-based equality.
/// Rapor asenkron işlendiğinden yalnızca talep kaydedilir.
/// </summary>
public record CreateReportRequest
{
    // Rapor talebi herhangi bir parametre gerektirmiyor;
    // sistem tüm lokasyonlar için otomatik rapor üretir.
}
