using ContactService.Domain.Enums;

namespace ContactService.Domain.Entities;

/// <summary>
/// Bir kişiye ait iletişim bilgisini (telefon, email, konum vb.) temsil eden entity.
/// BaseEntity'den UUID ve CreatedAt alanlarını miras alır.
/// </summary>
public class ContactInfo : BaseEntity
{
    public ContactInfoType InfoType { get; set; }
    public string InfoContent { get; set; } = string.Empty;

    public Guid PersonUUID { get; set; }
    public Person Person { get; set; } = null!;
}