using MassTransit;
using Microsoft.EntityFrameworkCore;
using ReportService.Application.Interfaces.Repositories;
using ReportService.Application.Interfaces.Services;
using ReportService.Infrastructure.Contexts;
using ReportService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IReportRepository, ReportRepository>();

builder.Services.AddScoped<IReportService, ReportService.Infrastructure.Services.ReportService>();

// MassTransit — Publisher rolü, mesaj yayınlar
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

