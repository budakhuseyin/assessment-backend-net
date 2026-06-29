using System;
using ContactService.Domain.Enums;

namespace ContactService.Domain.Entities;

public class ContactInfo
{
    public Guid UUID { get; set; }
    public ContactInfoType InfoType { get; set; } 
    public string InfoContent { get; set; }       
    
    public Guid PersonUUID { get; set; }
    public Person Person { get; set; }
}