namespace ContactService.Application.DTOs;

/// <summary>
/// Kişi bilgilerini döndürmek için kullanılan yanıt modeli.
/// Record type: immutable, value-based equality.
/// </summary>
public record PersonResponse
{
    public Guid UUID { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Company { get; init; }
    public List<ContactInfoResponse> ContactInfos { get; init; } = new();
}
