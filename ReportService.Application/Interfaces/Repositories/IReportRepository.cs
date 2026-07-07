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
}
