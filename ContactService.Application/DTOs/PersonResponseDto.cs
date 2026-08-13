namespace ContactService.Application.DTOs;

public record PersonResponseDto
{
    public Guid UUID { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
    public List<ContactInfoDto> ContactInfos { get; init; } = new();
}

public record ContactInfoDto
{
    public Guid UUID { get; init; }
    public string InfoType { get; init; } = string.Empty;
    public string InfoContent { get; init; } = string.Empty;
}
