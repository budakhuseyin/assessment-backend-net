using ContactService.API.Middleware;
using ContactService.Application.Interfaces.Repositories;
using ContactService.Application.Interfaces.Services;
using ContactService.Infrastructure.Consumers;
using ContactService.Infrastructure.Contexts;
using ContactService.Infrastructure.Repositories;
using ContactService.Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Serilog — appsettings.json'dan yapılandırmayı okuyarak loglama sistemini başlat
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));

// Redis — Dağıtık önbellek (Distributed Cache) kaydı
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "ContactService:";
});

// Rate Limiting — IP bazlı istek sınırlama (60 istek/dakika)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 60;                        // 1 dakikada max 60 istek
        limiterOptions.Window = TimeSpan.FromMinutes(1);        // Pencere süresi: 1 dakika
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;                          // Kuyruk yok, aşan istekler anında reddedilir
    });
    options.RejectionStatusCode = 429; // Too Many Requests
});

// Health Checks — PostgreSQL, Redis ve RabbitMQ bağlantı durumu kontrolü
var rabbitMqHostForHealth = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("PostgreSql")!,
        name: "postgresql",
        tags: new[] { "db", "ready" })
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379",
        name: "redis",
        tags: new[] { "cache", "ready" })
    .AddRabbitMQ(
        rabbitConnectionString: $"amqp://guest:guest@{rabbitMqHostForHealth}:5672",
        name: "rabbitmq",
        tags: new[] { "messaging", "ready" });


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IContactInfoRepository, ContactInfoRepository>();

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IContactInfoService, ContactInfoService>();

// ReportService'e HTTP isteği göndermek için — base URL appsettings'ten okunur
builder.Services.AddHttpClient("ReportService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ReportServiceUrl"] ?? "http://localhost:5063");
});

// MassTransit — Consumer rolü, ReportRequestedEvent'i dinler
var rabbitMqHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReportRequestedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uygulama başlarken migration'ları otomatik çalıştır (Docker container desteği için)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContactDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>(); // Önce hata yakalama
app.UseMiddleware<CorrelationIdMiddleware>();            // Sonra correlation ID
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("fixed");

// Health Check endpoint'i — /health adresinden servis durumunu döndür
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });
        await context.Response.WriteAsync(result);
    }
});





app.Run();

