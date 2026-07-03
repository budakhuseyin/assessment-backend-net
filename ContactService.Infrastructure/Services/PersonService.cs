using ContactService.Application.DTOs;
using ContactService.Application.Interfaces.Repositories;
using ContactService.Application.Interfaces.Services;
using ContactService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ContactService.Infrastructure.Contexts;

namespace ContactService.Infrastructure.Services;

/// <summary>
/// Kişi yönetimine ait iş kurallarını uygulayan servis sınıfı.
/// </summary>
public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly ContactDbContext _context;

    public PersonService(IPersonRepository personRepository, ContactDbContext context)
    {
        _personRepository = personRepository;
        _context = context;
    }

    public async Task<IEnumerable<PersonResponse>> GetAllAsync()
    {
        // ContactInfos ilişkisini de yükleyerek tüm kişileri getir
        var persons = await _context.Persons
            .Include(p => p.ContactInfos)
            .ToListAsync();

        return persons.Select(MapToResponse);
    }

    public async Task<PersonResponse?> GetByIdAsync(Guid id)
    {
        var person = await _context.Persons
            .Include(p => p.ContactInfos)
            .FirstOrDefaultAsync(p => p.UUID == id);

        return person == null ? null : MapToResponse(person);
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

        return MapToResponse(person);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var person = await _personRepository.GetByIdAsync(id);
        if (person == null) return false;

        await _personRepository.DeleteAsync(id);
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
