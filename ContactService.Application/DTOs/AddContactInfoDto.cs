using ContactService.Domain.Enums;

namespace ContactService.Application.DTOs;

public record AddContactInfoDto
{
    public Guid PersonUUID { get; init; }
    public ContactInfoType InfoType { get; init; }
    public string InfoContent { get; init; } = string.Empty;
}
