using ContactService.Application.Interfaces.Repositories;
using ContactService.Domain.Entities;
using ContactService.Infrastructure.Contexts;

namespace ContactService.Infrastructure.Repositories;

public class ContactInfoRepository : GenericRepository<ContactInfo>, IContactInfoRepository
{
    public ContactInfoRepository(ContactDbContext context) : base(context)
    {
    }
}
