using System;
using System.Collections.Generic;

namespace ContactService.Domain.Entities;

public class Person
{
    public Guid UUID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Company { get; set; }

    public ICollection<ContactInfo> ContactInfos { get; set; }

    public Person()
    {
        ContactInfos = new List<ContactInfo>();
    }
}