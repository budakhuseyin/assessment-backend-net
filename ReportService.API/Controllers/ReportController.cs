using Microsoft.AspNetCore.Mvc;
using ReportService.Application.DTOs;
using ReportService.Application.Interfaces.Services;

namespace ReportService.API.Controllers;

/// <summary>
/// Rapor yönetimi için HTTP endpoint'leri sağlayan controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Tüm raporları listeler.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reports = await _reportService.GetAllAsync();
        return Ok(reports);
    }

    /// <summary>
    /// UUID ile tek bir raporun tüm detaylarını getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var report = await _reportService.GetByIdAsync(id);
        if (report == null) return NotFound();
        return Ok(report);
    }

    /// <summary>
    /// Yeni bir rapor talebi oluşturur.
    /// Rapor "Preparing" statüsünde başlar, asenkron olarak tamamlanır.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var created = await _reportService.CreateReportAsync();
        return CreatedAtAction(nameof(GetById), new { id = created.UUID }, created);
    }

    /// <summary>
    /// ContactService tarafından çağrılır; raporu tamamlar ve detayları kaydeder.
    /// </summary>
    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteReportRequest request)
    {
        var success = await _reportService.CompleteReportAsync(id, request);
        if (!success) return NotFound();
        return NoContent();
    }
}

