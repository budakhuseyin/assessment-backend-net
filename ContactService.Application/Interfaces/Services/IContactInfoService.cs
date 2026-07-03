using ContactService.Application.DTOs;

namespace ContactService.Application.Interfaces.Services;

/// <summary>
/// İletişim bilgisi yönetimine ait iş kurallarını tanımlayan servis arayüzü.
/// </summary>
public interface IContactInfoService
{
    Task<ContactInfoResponse> AddContactInfoAsync(Guid personId, CreateContactInfoRequest request);
    Task<bool> DeleteContactInfoAsync(Guid contactInfoId);
}
