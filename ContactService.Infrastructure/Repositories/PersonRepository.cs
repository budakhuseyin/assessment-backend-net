using ContactService.Application.Interfaces.Repositories;
using ContactService.Domain.Entities;
using ContactService.Infrastructure.Contexts;

namespace ContactService.Infrastructure.Repositories;

public class PersonRepository : GenericRepository<Person>, IPersonRepository
{
    public PersonRepository(ContactDbContext context) : base(context)
    {
    }
}
