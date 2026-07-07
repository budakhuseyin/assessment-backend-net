using Microsoft.EntityFrameworkCore;
using ReportService.Domain.Entities;

namespace ReportService.Infrastructure.Contexts;

/// <summary>
/// Raporlama servisi için EF Core veritabanı bağlamı.
/// </summary>
public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
    {
    }

    public DbSet<Report> Reports { get; set; }
    public DbSet<ReportDetail> ReportDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Report için birincil anahtar tanımı
        modelBuilder.Entity<Report>()
            .HasKey(r => r.UUID);

        // ReportDetail için birincil anahtar tanımı
        modelBuilder.Entity<ReportDetail>()
            .HasKey(rd => rd.UUID);

        // Report → ReportDetail bire-çok ilişkisi (Cascade Delete)
        modelBuilder.Entity<Report>()
            .HasMany(r => r.ReportDetails)
            .WithOne(rd => rd.Report)
            .HasForeignKey(rd => rd.ReportUUID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
