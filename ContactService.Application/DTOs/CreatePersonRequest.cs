namespace ContactService.Application.DTOs;

/// <summary>
/// Yeni bir kişi oluşturmak için kullanılan istek modeli.
/// Record type: immutable, value-based equality, JSON deserializasyon ile uyumlu.
/// </summary>
public record CreatePersonRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
}
