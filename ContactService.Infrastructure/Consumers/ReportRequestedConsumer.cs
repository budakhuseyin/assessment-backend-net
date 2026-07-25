using ContactService.Domain.Enums;
using ContactService.Infrastructure.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Messages.Events;
using System.Net.Http.Json;

namespace ContactService.Infrastructure.Consumers;

/// <summary>
/// ReportService'ten gelen ReportRequestedEvent'i dinleyen MassTransit consumer sınıfı.
/// ContactService veritabanından lokasyon bazlı istatistikleri hesaplar ve
/// ReportService API'sine geri bildirim yaparak raporu tamamlar.
/// </summary>
public class ReportRequestedConsumer : IConsumer<ReportRequestedEvent>
{
    private readonly ContactDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public ReportRequestedConsumer(ContactDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task Consume(ConsumeContext<ReportRequestedEvent> context)
    {
        var reportId = context.Message.ReportId;

        var persons = await _context.Persons
            .Include(p => p.ContactInfos)
            .ToListAsync();

        // Adres bazında gruplandır; her lokasyon için kişi ve telefon sayısını hesapla
        var locationGroups = persons
            .SelectMany(p => p.ContactInfos
                .Where(c => c.InfoType == ContactInfoType.Address)
                .Select(c => new { Location = c.InfoContent, Person = p }))
            .GroupBy(x => x.Location)
            .Select(g => new
            {
                Location = g.Key,
                PersonCount = g.Select(x => x.Person.UUID).Distinct().Count(),
                PhoneNumberCount = g.Select(x => x.Person)
                    .SelectMany(p => p.ContactInfos
                        .Where(c => c.InfoType == ContactInfoType.Phone))
                    .Count()
            })
            .ToList();

        var client = _httpClientFactory.CreateClient("ReportService");
        var payload = new
        {
            Details = locationGroups.Select(g => new
            {
                g.Location,
                g.PersonCount,
                g.PhoneNumberCount
            }).ToList()
        };

        await client.PutAsJsonAsync($"/api/report/{reportId}/complete", payload);
    }
}

