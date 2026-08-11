using ContactService.Infrastructure.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace ContactService.IntegrationTests;

/// <summary>
/// Integration testleri için ContactService'i InMemory veritabanı ile
/// ayağa kaldıran özel WebApplicationFactory sınıfı.
/// Gerçek PostgreSQL, Redis ve RabbitMQ bağlantısına ihtiyaç duymaz.
/// </summary>
public class ContactServiceWebApplicationFactory : WebApplicationFactory<Program>
{
    // Tüm testlerin aynı InMemory DB'ye bakabilmesi için sabit bir isim kullanılır.
    // Guid, factory oluşturulduğunda bir kez üretilir; her request'te değil.
    private readonly string _dbName = "ContactTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Gerçek PostgreSQL bağlantısını kaldır, InMemory ile değiştir
            services.RemoveAll<DbContextOptions<ContactDbContext>>();
            services.AddDbContext<ContactDbContext>(options =>
                options.UseInMemoryDatabase(_dbName)); // Tüm requestler aynı DB'yi kullanır

            // Redis'i InMemory ile değiştir (test ortamında Redis gerekmez)
            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();
        });

        builder.UseEnvironment("Testing");
    }
}
