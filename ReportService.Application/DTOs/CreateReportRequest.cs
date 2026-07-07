namespace ReportService.Application.DTOs;

/// <summary>
/// Rapor oluşturma talebini temsil eden istek modeli.
/// Rapor asenkron işlendiğinden yalnızca talep kaydedilir.
/// </summary>
public class CreateReportRequest
{
    // Rapor talebi herhangi bir parametre gerektirmiyor;
    // sistem tüm lokasyonlar için otomatik rapor üretir.
}
