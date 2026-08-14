using Microsoft.AspNetCore.Mvc;

namespace ReportService.API.Controllers;

/// <summary>
/// Tüm ReportService controller'ları için ortak davranışları ve ayarları tanımlayan temel controller.
/// [ApiController] ve [Route] nitelikleri burada merkezi olarak tanımlanır.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Başarılı bir kaynağı 200 OK ile döner.
    /// </summary>
    protected IActionResult Success<T>(T data) => Ok(data);

    /// <summary>
    /// Kaynak bulunamadığında standart 404 NotFound döner.
    /// </summary>
    protected IActionResult NotFoundResult(string message = "Kaynak bulunamadı.")
        => NotFound(new { message });

    /// <summary>
    /// Yeni oluşturulan kaynağı 201 Created ile döner.
    /// </summary>
    protected IActionResult Created<T>(string actionName, object routeValues, T data)
        => CreatedAtAction(actionName, routeValues, data);
}
