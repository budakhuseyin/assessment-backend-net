using Microsoft.EntityFrameworkCore;
using ReportService.Application.Interfaces.Repositories;
using ReportService.Application.Interfaces.Services;
using ReportService.Infrastructure.Contexts;
using ReportService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controller desteğini ekle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with PostgreSQL
builder.Services.AddDbContext<ReportDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));
});

// Configure Dependency Injection for Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Configure Dependency Injection for Services
builder.Services.AddScoped<IReportService, ReportService.Infrastructure.Services.ReportService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
