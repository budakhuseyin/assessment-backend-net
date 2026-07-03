namespace ContactService.Application.DTOs;

/// <summary>
/// Yeni bir kişi oluşturmak için kullanılan istek modeli.
/// </summary>
public class CreatePersonRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
}
