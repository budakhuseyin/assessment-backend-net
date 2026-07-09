using ContactService.Application.DTOs;
using ContactService.Application.Interfaces.Repositories;
using ContactService.Domain.Entities;
using ContactService.Infrastructure.Services;
using Moq;
using Xunit;

namespace ContactService.Tests.Services;

public class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _mockPersonRepository;
    private readonly PersonService _personService;

    public PersonServiceTests()
    {
        // 1. Arrange (Hazırlık)
        // Gerçek bir veritabanı kullanmak yerine Moq kütüphanesi ile "sahte (mock)" bir repository oluşturuyoruz.
        _mockPersonRepository = new Mock<IPersonRepository>();
        
        // Servisimizi bu sahte repository ile ayağa kaldırıyoruz.
        // Not: CreateAsync metodu DbContext kullanmadığı için DbContext parametresine null geçiyoruz.
        _personService = new PersonService(_mockPersonRepository.Object, null!);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ShouldReturnPersonResponseWithSameData()
    {
        // 1. Arrange (Hazırlık)
        var request = new CreatePersonRequest
        {
            FirstName = "Ali",
            LastName = "Yılmaz",
            Company = "Aras Kargo"
        };

        // Repository'nin AddAsync metodu çağrıldığında hiçbir şey yapmadan başarılı dönmesini (CompletedTask) söylüyoruz.
        _mockPersonRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Person>()))
            .Returns(Task.CompletedTask);

        // 2. Act (Eylem)
        // Test etmek istediğimiz asıl metodu çağırıyoruz.
        var result = await _personService.CreateAsync(request);

        // 3. Assert (Doğrulama)
        // Metodun döndüğü sonucun, bizim gönderdiğimiz request ile aynı olup olmadığını kontrol ediyoruz.
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.UUID); // Guid üretilmiş olmalı
        Assert.Equal(request.FirstName, result.FirstName);
        Assert.Equal(request.LastName, result.LastName);
        Assert.Equal(request.Company, result.Company);
        
        // AddAsync metodunun tam olarak 1 kere çağrıldığından emin oluyoruz.
        _mockPersonRepository.Verify(repo => repo.AddAsync(It.IsAny<Person>()), Times.Once);
    }
}
