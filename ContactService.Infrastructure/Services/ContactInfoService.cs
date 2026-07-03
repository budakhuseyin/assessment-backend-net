using ContactService.Application.DTOs;
using ContactService.Application.Interfaces.Repositories;
using ContactService.Application.Interfaces.Services;
using ContactService.Domain.Entities;

namespace ContactService.Infrastructure.Services;

/// <summary>
/// İletişim bilgisi yönetimine ait iş kurallarını uygulayan servis sınıfı.
/// </summary>
public class ContactInfoService : IContactInfoService
{
    private readonly IContactInfoRepository _contactInfoRepository;
    private readonly IPersonRepository _personRepository;

    public ContactInfoService(
        IContactInfoRepository contactInfoRepository,
        IPersonRepository personRepository)
    {
        _contactInfoRepository = contactInfoRepository;
        _personRepository = personRepository;
    }

    public async Task<ContactInfoResponse> AddContactInfoAsync(Guid personId, CreateContactInfoRequest request)
    {
        // Kişinin var olup olmadığını kontrol et
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"UUID: {personId} olan kişi bulunamadı.");

        var contactInfo = new ContactInfo
        {
            UUID = Guid.NewGuid(),
            InfoType = request.InfoType,
            InfoContent = request.InfoContent,
            PersonUUID = personId
        };

        await _contactInfoRepository.AddAsync(contactInfo);

        return new ContactInfoResponse
        {
            UUID = contactInfo.UUID,
            InfoType = contactInfo.InfoType,
            InfoContent = contactInfo.InfoContent
        };
    }

    public async Task<bool> DeleteContactInfoAsync(Guid contactInfoId)
    {
        var contactInfo = await _contactInfoRepository.GetByIdAsync(contactInfoId);
        if (contactInfo == null) return false;

        await _contactInfoRepository.DeleteAsync(contactInfoId);
        return true;
    }
}
