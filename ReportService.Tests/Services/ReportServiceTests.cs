using Moq;
using ReportService.Application.Interfaces.Repositories;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using Xunit;

namespace ReportService.Tests.Services;

public class ReportServiceTests
{
    private readonly Mock<IReportRepository> _mockReportRepository;
    private readonly ReportService.Infrastructure.Services.ReportService _reportService;

    public ReportServiceTests()
    {
        _mockReportRepository = new Mock<IReportRepository>();
        _reportService = new ReportService.Infrastructure.Services.ReportService(_mockReportRepository.Object);
    }

    // ─────────────────────────── CreateReportAsync Testleri ───────────────────────────

    [Fact]
    public async Task CreateReportAsync_ShouldReturnReportWithPreparingStatus()
    {
        // 1. Arrange
        _mockReportRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Report>()))
            .Returns(Task.CompletedTask);

        // 2. Act
        var result = await _reportService.CreateReportAsync();

        // 3. Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.UUID);               // Benzersiz UUID üretilmeli
        Assert.Equal(ReportStatus.Preparing, result.Status);     // Başlangıç statüsü Preparing olmalı
        Assert.Empty(result.ReportDetails);                       // Yeni raporda henüz detay olmamalı

        // AddAsync tam olarak 1 kere çağrılmış olmalı
        _mockReportRepository.Verify(repo => repo.AddAsync(It.IsAny<Report>()), Times.Once);
    }

    [Fact]
    public async Task CreateReportAsync_ShouldSetRequestedAtToUtcNow()
    {
        // 1. Arrange
        var beforeCall = DateTime.UtcNow;

        _mockReportRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Report>()))
            .Returns(Task.CompletedTask);

        // 2. Act
        var result = await _reportService.CreateReportAsync();
        var afterCall = DateTime.UtcNow;

        // 3. Assert
        // RequestedAt, metod çağrılmadan önce ve sonra arasında bir zaman olmalı
        Assert.True(result.RequestedAt >= beforeCall && result.RequestedAt <= afterCall);
    }

    // ─────────────────────────── GetAllAsync Testleri ───────────────────────────

    [Fact]
    public async Task GetAllAsync_WithReports_ShouldReturnMappedResponses()
    {
        // 1. Arrange
        var fakeReports = new List<Report>
        {
            new() { UUID = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Completed },
            new() { UUID = Guid.NewGuid(), RequestedAt = DateTime.UtcNow, Status = ReportStatus.Preparing }
        };

        _mockReportRepository
            .Setup(repo => repo.GetAllWithDetailsAsync())
            .ReturnsAsync(fakeReports);

        // 2. Act
        var result = await _reportService.GetAllAsync();

        // 3. Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);                              // 2 rapor dönmeli
        Assert.Equal(ReportStatus.Completed, resultList[0].Status);
        Assert.Equal(ReportStatus.Preparing, resultList[1].Status);
    }

    [Fact]
    public async Task GetAllAsync_WithNoReports_ShouldReturnEmptyList()
    {
        // 1. Arrange
        _mockReportRepository
            .Setup(repo => repo.GetAllWithDetailsAsync())
            .ReturnsAsync(new List<Report>());

        // 2. Act
        var result = await _reportService.GetAllAsync();

        // 3. Assert
        Assert.Empty(result); // Hiç rapor yoksa boş liste dönmeli
    }

    // ─────────────────────────── GetByIdAsync Testleri ───────────────────────────

    [Fact]
    public async Task GetByIdAsync_ExistingReport_ShouldReturnReportResponse()
    {
        // 1. Arrange
        var existingReport = new Report
        {
            UUID = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            Status = ReportStatus.Completed
        };

        _mockReportRepository
            .Setup(repo => repo.GetByIdWithDetailsAsync(existingReport.UUID))
            .ReturnsAsync(existingReport);

        // 2. Act
        var result = await _reportService.GetByIdAsync(existingReport.UUID);

        // 3. Assert
        Assert.NotNull(result);
        Assert.Equal(existingReport.UUID, result.UUID);
        Assert.Equal(ReportStatus.Completed, result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingReport_ShouldReturnNull()
    {
        // 1. Arrange
        var nonExistingId = Guid.NewGuid();

        _mockReportRepository
            .Setup(repo => repo.GetByIdWithDetailsAsync(nonExistingId))
            .ReturnsAsync((Report?)null);

        // 2. Act
        var result = await _reportService.GetByIdAsync(nonExistingId);

        // 3. Assert
        Assert.Null(result); // Rapor yoksa null dönmeli
    }
}
