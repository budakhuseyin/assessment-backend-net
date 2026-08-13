using ContactService.Domain.Enums;

namespace ContactService.Application.DTOs;

/// <summary>
/// Bir kişiye iletişim bilgisi eklemek için kullanılan istek modeli.
/// Record type: immutable, value-based equality.
/// </summary>
public record CreateContactInfoRequest
{
    public ContactInfoType InfoType { get; init; }
    public string InfoContent { get; init; } = string.Empty;
}
