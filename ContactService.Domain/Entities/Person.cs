namespace ContactService.Domain.Entities;

/// <summary>
/// Rehberdeki bir kişiyi temsil eden entity.
/// BaseEntity'den UUID ve CreatedAt alanlarını miras alır.
/// </summary>
public class Person : BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Company { get; set; }

    public ICollection<ContactInfo> ContactInfos { get; set; }

    public Person()
    {
        ContactInfos = new List<ContactInfo>();
    }
}