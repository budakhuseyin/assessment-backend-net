using ContactService.Application.Interfaces.Repositories;
using ContactService.Application.Interfaces.Services;
using ContactService.Infrastructure.Consumers;
using ContactService.Infrastructure.Contexts;
using ContactService.Infrastructure.Repositories;
using ContactService.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));

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

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

