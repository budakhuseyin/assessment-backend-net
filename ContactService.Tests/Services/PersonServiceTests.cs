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
        // Gerçek bir veritabanı kullanmak yerine Moq kütüphanesi ile "sahte (mock)" bir repository oluşturuyoruz.
        _mockPersonRepository = new Mock<IPersonRepository>();

        // Servisimizi bu sahte repository ile ayağa kaldırıyoruz.
        // Not: GetAllAsync ve GetByIdAsync DbContext kullandığı için sadece bu metodları test ederken
        // repository mock'u yeterli, DbContext'e ihtiyaç yok. Bu yüzden null! geçiyoruz.
        _personService = new PersonService(_mockPersonRepository.Object, null!);
    }

    // ─────────────────────────── CreateAsync Testleri ───────────────────────────

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

        _mockPersonRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Person>()))
            .Returns(Task.CompletedTask);

        // 2. Act (Eylem)
        var result = await _personService.CreateAsync(request);

        // 3. Assert (Doğrulama)
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.UUID);       // Benzersiz bir UUID üretilmeli
        Assert.Equal(request.FirstName, result.FirstName);
        Assert.Equal(request.LastName, result.LastName);
        Assert.Equal(request.Company, result.Company);

        // AddAsync tam olarak 1 kere çağrılmış olmalı
        _mockPersonRepository.Verify(repo => repo.AddAsync(It.IsAny<Person>()), Times.Once);
    }

    // ─────────────────────────── DeleteAsync Testleri ───────────────────────────

    [Fact]
    public async Task DeleteAsync_ExistingPerson_ShouldReturnTrue()
    {
        // 1. Arrange
        // Var olduğunu simüle edeceğimiz kişiyi hazırlıyoruz.
        var existingPerson = new Person
        {
            UUID = Guid.NewGuid(),
            FirstName = "Mehmet",
            LastName = "Demir",
            Company = "Aras Kargo"
        };

        // Bu UUID sorgulandığında kişiyi döndür.
        _mockPersonRepository
            .Setup(repo => repo.GetByIdAsync(existingPerson.UUID))
            .ReturnsAsync(existingPerson);

        _mockPersonRepository
            .Setup(repo => repo.DeleteAsync(existingPerson.UUID))
            .Returns(Task.CompletedTask);

        // 2. Act
        var result = await _personService.DeleteAsync(existingPerson.UUID);

        // 3. Assert
        Assert.True(result); // Kişi bulundu ve silindi → true dönmeli

        // DeleteAsync tam olarak 1 kere çağrılmış olmalı
        _mockPersonRepository.Verify(repo => repo.DeleteAsync(existingPerson.UUID), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingPerson_ShouldReturnFalse()
    {
        // 1. Arrange
        var nonExistingId = Guid.NewGuid();

        // Bu UUID için null döndür → kişi veritabanında yok.
        _mockPersonRepository
            .Setup(repo => repo.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Person?)null);

        // 2. Act
        var result = await _personService.DeleteAsync(nonExistingId);

        // 3. Assert
        Assert.False(result); // Kişi bulunamadı → false dönmeli

        // Kişi olmadığı için DeleteAsync hiç çağrılmamalıydı.
        _mockPersonRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
