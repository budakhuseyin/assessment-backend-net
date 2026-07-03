namespace ContactService.Application.DTOs;

/// <summary>
/// Kişi bilgilerini döndürmek için kullanılan yanıt modeli.
/// </summary>
public class PersonResponse
{
    public Guid UUID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Company { get; set; }
    public List<ContactInfoResponse> ContactInfos { get; set; } = new();
}
