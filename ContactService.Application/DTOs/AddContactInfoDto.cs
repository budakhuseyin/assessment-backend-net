using ContactService.Domain.Enums;

namespace ContactService.Application.DTOs;

public class AddContactInfoDto
{
    public Guid PersonUUID { get; set; }
    public ContactInfoType InfoType { get; set; }
    public string InfoContent { get; set; } = string.Empty;
}
