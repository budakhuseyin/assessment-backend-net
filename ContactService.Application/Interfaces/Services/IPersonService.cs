using ContactService.Application.DTOs;

namespace ContactService.Application.Interfaces.Services;

/// <summary>
/// Kişi yönetimine ait iş kurallarını tanımlayan servis arayüzü.
/// </summary>
public interface IPersonService
{
    Task<IEnumerable<PersonResponse>> GetAllAsync();
    Task<PersonResponse?> GetByIdAsync(Guid id);
    Task<PersonResponse> CreateAsync(CreatePersonRequest request);
    Task<bool> DeleteAsync(Guid id);
}
