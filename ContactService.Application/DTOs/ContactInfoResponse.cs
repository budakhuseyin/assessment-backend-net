using ContactService.Domain.Enums;

namespace ContactService.Application.DTOs;

/// <summary>
/// İletişim bilgisini döndürmek için kullanılan yanıt modeli.
/// Record type: immutable, value-based equality.
/// </summary>
public record ContactInfoResponse
{
    public Guid UUID { get; init; }
    public ContactInfoType InfoType { get; init; }
    public string? InfoContent { get; init; }
}
