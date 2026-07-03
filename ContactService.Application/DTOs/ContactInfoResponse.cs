using ContactService.Domain.Enums;

namespace ContactService.Application.DTOs;

/// <summary>
/// İletişim bilgisini döndürmek için kullanılan yanıt modeli.
/// </summary>
public class ContactInfoResponse
{
    public Guid UUID { get; set; }
    public ContactInfoType InfoType { get; set; }
    public string? InfoContent { get; set; }
}
