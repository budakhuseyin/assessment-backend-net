using ReportService.Domain.Entities;

namespace ReportService.Application.Interfaces.Repositories;

/// <summary>
/// Rapor entity'sine özgü veri erişim işlemlerini tanımlayan arayüz.
/// </summary>
public interface IReportRepository : IGenericRepository<Report>
{
    /// <summary>
    /// Raporu, ilişkili ReportDetail'lar dahil olmak üzere getirir.
    /// </summary>
    Task<Report?> GetByIdWithDetailsAsync(Guid id);

    /// <summary>
    /// Tüm raporları listeler.
    /// </summary>
    Task<IEnumerable<Report>> GetAllWithDetailsAsync();

    /// <summary>
    /// Raporu tamamlanmış olarak işaretler ve detayları kaydeder.
    /// ReportDetail'lar doğrudan DbSet üzerinden eklenir (EF Core tracking sorunu önlenir).
    /// </summary>
    Task<bool> CompleteAsync(Guid reportId, IEnumerable<ReportDetail> newDetails);
}
