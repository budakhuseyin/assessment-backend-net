namespace Shared.Messages.Events;

/// <summary>
/// ReportService tarafından yayınlanan rapor talebi eventi.
/// ContactService bu eventi dinleyerek lokasyon bazlı istatistikleri hesaplar
/// ve raporu günceller.
/// </summary>
public class ReportRequestedEvent
{
    /// <summary>
    /// Güncellenmesi gereken raporun benzersiz kimliği.
    /// </summary>
    public Guid ReportId { get; set; }
}
