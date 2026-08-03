using ContactService.Application.DTOs;
using ContactService.Application.Interfaces.Repositories;
using ContactService.Application.Interfaces.Services;
using ContactService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ContactService.Infrastructure.Contexts;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ContactService.Infrastructure.Services;

/// <summary>
/// Kişi yönetimine ait iş kurallarını uygulayan servis sınıfı.
/// Cache-Aside pattern kullanılarak sık okunan veriler Redis'te önbelleğe alınır.
/// </summary>
public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly ContactDbContext _context;
    private readonly IDistributedCache _cache;

    // Cache anahtar sabitleri
    private const string AllPersonsCacheKey = "all_persons";
    private static string PersonByIdCacheKey(Guid id) => $"person_{id}";

    // Cache süresi: 5 dakika
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public PersonService(IPersonRepository personRepository, ContactDbContext context, IDistributedCache cache)
    {
        _personRepository = personRepository;
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<PersonResponse>> GetAllAsync()
    {
        // 1. Önce Redis'e bak
        var cached = await _cache.GetStringAsync(AllPersonsCacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<IEnumerable<PersonResponse>>(cached)!;

        // 2. Redis'te yoksa veritabanına git
        var persons = await _context.Persons
            .Include(p => p.ContactInfos)
            .ToListAsync();

        var response = persons.Select(MapToResponse).ToList();

        // 3. Sonucu Redis'e yaz (sonraki istekler için)
        await _cache.SetStringAsync(AllPersonsCacheKey,
            JsonSerializer.Serialize(response), CacheOptions);

        return response;
    }

    public async Task<PersonResponse?> GetByIdAsync(Guid id)
    {
        var cacheKey = PersonByIdCacheKey(id);

        // 1. Önce Redis'e bak
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<PersonResponse>(cached);

        // 2. Redis'te yoksa veritabanına git
        var person = await _context.Persons
            .Include(p => p.ContactInfos)
            .FirstOrDefaultAsync(p => p.UUID == id);

        if (person == null) return null;

        var response = MapToResponse(person);

        // 3. Sonucu Redis'e yaz
        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(response), CacheOptions);

        return response;
    }

    public async Task<PersonResponse> CreateAsync(CreatePersonRequest request)
    {
        var person = new Person
        {
            UUID = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Company = request.Company
        };

        await _personRepository.AddAsync(person);

        // Cache Invalidation: Yeni kişi eklenince liste cache'i geçersiz kıl
        await _cache.RemoveAsync(AllPersonsCacheKey);

        return MapToResponse(person);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var person = await _personRepository.GetByIdAsync(id);
        if (person == null) return false;

        await _personRepository.DeleteAsync(id);

        // Cache Invalidation: Silinen kişinin ve listenin cache'ini temizle
        await _cache.RemoveAsync(AllPersonsCacheKey);
        await _cache.RemoveAsync(PersonByIdCacheKey(id));

        return true;
    }

    // Entity'den DTO'ya dönüşüm (private yardımcı metod)
    private static PersonResponse MapToResponse(Person person) => new()
    {
        UUID = person.UUID,
        FirstName = person.FirstName,
        LastName = person.LastName,
        Company = person.Company,
        ContactInfos = person.ContactInfos.Select(c => new ContactInfoResponse
        {
            UUID = c.UUID,
            InfoType = c.InfoType,
            InfoContent = c.InfoContent
        }).ToList()
    };
}
