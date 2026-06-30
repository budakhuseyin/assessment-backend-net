namespace ContactService.Application.DTOs;

public class PersonResponseDto
{
    public Guid UUID { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public List<ContactInfoDto> ContactInfos { get; set; } = new();
}

public class ContactInfoDto
{
    public Guid UUID { get; set; }
    public string InfoType { get; set; } = string.Empty;
    public string InfoContent { get; set; } = string.Empty;
}
