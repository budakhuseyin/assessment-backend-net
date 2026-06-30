using ContactService.Domain.Entities;

namespace ContactService.Application.Interfaces;

public interface IPersonRepository
{
    Task<IEnumerable<Person>> GetAllAsync();
    Task<Person> GetByIdAsync(Guid uuid);
    Task<Person> AddAsync(Person person);
    Task DeleteAsync(Guid uuid);
}