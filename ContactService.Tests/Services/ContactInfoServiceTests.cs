using ContactService.Application.DTOs;
using ContactService.Application.Interfaces.Repositories;
using ContactService.Domain.Entities;
using ContactService.Domain.Enums;
using ContactService.Infrastructure.Services;
using Moq;
using Xunit;

namespace ContactService.Tests.Services;

public class ContactInfoServiceTests
{
    private readonly Mock<IContactInfoRepository> _mockContactInfoRepository;
    private readonly Mock<IPersonRepository> _mockPersonRepository;
    private readonly ContactInfoService _contactInfoService;

    public ContactInfoServiceTests()
    {
        _mockContactInfoRepository = new Mock<IContactInfoRepository>();
        _mockPersonRepository = new Mock<IPersonRepository>();

        _contactInfoService = new ContactInfoService(
            _mockContactInfoRepository.Object,
            _mockPersonRepository.Object
        );
    }

    // ─────────────────── AddContactInfoAsync Testleri ───────────────────

    [Fact]
    public async Task AddContactInfoAsync_ExistingPerson_ShouldReturnContactInfoResponse()
    {
        // 1. Arrange
        var personId = Guid.NewGuid();
        var existingPerson = new Person
        {
            UUID = personId,
            FirstName = "Ali",
            LastName = "Yılmaz",
            Company = "Aras Kargo"
        };

        var request = new CreateContactInfoRequest
        {
            InfoType = ContactInfoType.Phone,
            InfoContent = "05321234567"
        };

        // Kişi var → döndür
        _mockPersonRepository
            .Setup(repo => repo.GetByIdAsync(personId))
            .ReturnsAsync(existingPerson);

        // İletişim bilgisi ekleme → başarılı
        _mockContactInfoRepository
            .Setup(repo => repo.AddAsync(It.IsAny<ContactInfo>()))
            .Returns(Task.CompletedTask);

        // 2. Act
        var result = await _contactInfoService.AddContactInfoAsync(personId, request);

        // 3. Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.UUID);         // Yeni UUID üretilmeli
        Assert.Equal(request.InfoType, result.InfoType);
        Assert.Equal(request.InfoContent, result.InfoContent);

        // AddAsync tam olarak 1 kere çağrılmış olmalı
        _mockContactInfoRepository.Verify(repo => repo.AddAsync(It.IsAny<ContactInfo>()), Times.Once);
    }

    [Fact]
    public async Task AddContactInfoAsync_NonExistingPerson_ShouldThrowKeyNotFoundException()
    {
        // 1. Arrange
        var nonExistingPersonId = Guid.NewGuid();

        var request = new CreateContactInfoRequest
        {
            InfoType = ContactInfoType.Email,
            InfoContent = "ali@arasnot.com"
        };

        // Kişi yok → null döndür
        _mockPersonRepository
            .Setup(repo => repo.GetByIdAsync(nonExistingPersonId))
            .ReturnsAsync((Person?)null);

        // 2. Act & Assert
        // Bu metod çağrıldığında KeyNotFoundException fırlatılmasını bekliyoruz.
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _contactInfoService.AddContactInfoAsync(nonExistingPersonId, request)
        );

        // Kişi olmadığı için iletişim bilgisi hiç eklenmemiş olmalı
        _mockContactInfoRepository.Verify(repo => repo.AddAsync(It.IsAny<ContactInfo>()), Times.Never);
    }

    // ─────────────────── DeleteContactInfoAsync Testleri ───────────────────

    [Fact]
    public async Task DeleteContactInfoAsync_ExistingContactInfo_ShouldReturnTrue()
    {
        // 1. Arrange
        var existingContactInfo = new ContactInfo
        {
            UUID = Guid.NewGuid(),
            InfoType = ContactInfoType.Phone,
            InfoContent = "05321234567",
            PersonUUID = Guid.NewGuid()
        };

        _mockContactInfoRepository
            .Setup(repo => repo.GetByIdAsync(existingContactInfo.UUID))
            .ReturnsAsync(existingContactInfo);

        _mockContactInfoRepository
            .Setup(repo => repo.DeleteAsync(existingContactInfo.UUID))
            .Returns(Task.CompletedTask);

        // 2. Act
        var result = await _contactInfoService.DeleteContactInfoAsync(existingContactInfo.UUID);

        // 3. Assert
        Assert.True(result); // Bulundu ve silindi → true

        _mockContactInfoRepository.Verify(repo => repo.DeleteAsync(existingContactInfo.UUID), Times.Once);
    }

    [Fact]
    public async Task DeleteContactInfoAsync_NonExistingContactInfo_ShouldReturnFalse()
    {
        // 1. Arrange
        var nonExistingId = Guid.NewGuid();

        _mockContactInfoRepository
            .Setup(repo => repo.GetByIdAsync(nonExistingId))
            .ReturnsAsync((ContactInfo?)null);

        // 2. Act
        var result = await _contactInfoService.DeleteContactInfoAsync(nonExistingId);

        // 3. Assert
        Assert.False(result); // Bulunamadı → false

        // Kayıt olmadığı için DeleteAsync hiç çağrılmamalı
        _mockContactInfoRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
