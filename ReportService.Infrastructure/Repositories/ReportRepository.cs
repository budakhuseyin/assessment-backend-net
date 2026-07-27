using Microsoft.EntityFrameworkCore;
using ReportService.Application.Interfaces.Repositories;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using ReportService.Infrastructure.Contexts;

namespace ReportService.Infrastructure.Repositories;

/// <summary>
/// Rapor entity'sine özgü veri erişim işlemlerini uygulayan repository sınıfı.
/// </summary>
public class ReportRepository : GenericRepository<Report>, IReportRepository
{
    public ReportRepository(ReportDbContext context) : base(context)
    {
    }

    public async Task<Report?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Reports
            .Include(r => r.ReportDetails)
            .FirstOrDefaultAsync(r => r.UUID == id);
    }

    public async Task<IEnumerable<Report>> GetAllWithDetailsAsync()
    {
        return await _context.Reports
            .Include(r => r.ReportDetails)
            .ToListAsync();
    }

    public async Task<bool> CompleteAsync(Guid reportId, IEnumerable<ReportDetail> newDetails)
    {
        var report = await _context.Reports.FirstOrDefaultAsync(r => r.UUID == reportId);
        if (report == null) return false;

        // Doğrudan DbSet üzerinden ekleme — EF Core bu nesneleri kesin "Added" olarak işaretler
        await _context.ReportDetails.AddRangeAsync(newDetails);
        report.Status = ReportStatus.Completed;
        await _context.SaveChangesAsync();
        return true;
    }
}

