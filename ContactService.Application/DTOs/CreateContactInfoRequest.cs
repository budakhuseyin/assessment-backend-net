using ContactService.Domain.Enums;

namespace ContactService.Application.DTOs;

/// <summary>
/// Bir kişiye iletişim bilgisi eklemek için kullanılan istek modeli.
/// </summary>
public class CreateContactInfoRequest
{
    public ContactInfoType InfoType { get; set; }
    public string InfoContent { get; set; } = string.Empty;
}
