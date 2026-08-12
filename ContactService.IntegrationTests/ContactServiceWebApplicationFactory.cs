using ContactService.Infrastructure.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContactService.IntegrationTests;

/// <summary>
/// Integration testleri için ContactService'i InMemory veritabanı ile
/// ayağa kaldıran özel WebApplicationFactory sınıfı.
/// Gerçek PostgreSQL, Redis ve RabbitMQ bağlantısına ihtiyaç duymaz.
/// </summary>
public class ContactServiceWebApplicationFactory : WebApplicationFactory<Program>
{
    // Tüm testlerin aynı InMemory DB'ye bakabilmesi için sabit bir isim kullanılır.
    private readonly string _dbName = "ContactTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Gerçek PostgreSQL bağlantısını kaldır, InMemory ile değiştir
            services.RemoveAll<DbContextOptions<ContactDbContext>>();
            services.AddDbContext<ContactDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Redis'i InMemory ile değiştir
            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();

            // MassTransit/RabbitMQ servislerini kaldır (GitHub Actions'da RabbitMQ yok)
            // IBus, IPublishEndpoint, ISendEndpointProvider gibi MassTransit servislerini temizle
            var massTransitDescriptors = services
                .Where(d => d.ServiceType.Namespace != null &&
                            d.ServiceType.Namespace.StartsWith("MassTransit"))
                .ToList();

            foreach (var descriptor in massTransitDescriptors)
                services.Remove(descriptor);

            // MassTransit'i InMemory test modu ile yeniden kaydet
            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) =>
                    cfg.ConfigureEndpoints(context));
            });
        });

        builder.UseEnvironment("Testing");
    }
}
