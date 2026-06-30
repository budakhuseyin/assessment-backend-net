namespace ContactService.Application.DTOs;

public class CreatePersonDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
}
