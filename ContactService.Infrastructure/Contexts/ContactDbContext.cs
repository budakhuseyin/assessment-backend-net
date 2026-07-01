using ContactService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactService.Infrastructure.Contexts;

public class ContactDbContext : DbContext
{
    public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<ContactInfo> ContactInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // One-to-Many relationship setup
        modelBuilder.Entity<Person>()
            .HasMany(p => p.ContactInfos)
            .WithOne(c => c.Person)
            .HasForeignKey(c => c.PersonUUID)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Person>()
            .HasKey(p => p.UUID);
            
        modelBuilder.Entity<ContactInfo>()
            .HasKey(c => c.UUID);
    }
}
