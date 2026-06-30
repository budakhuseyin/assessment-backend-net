using ContactService.Domain.Entities;

namespace ContactService.Application.Interfaces;

public interface IContactInfoRepository
{
    Task AddAsync(ContactInfo contactInfo);
    Task DeleteAsync(Guid uuid);
}
