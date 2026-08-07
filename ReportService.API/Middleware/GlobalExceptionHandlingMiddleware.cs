using System.Net;
using System.Text.Json;

namespace ReportService.API.Middleware;

/// <summary>
/// Uygulama genelinde yakalanmamış tüm exception'ları merkezi olarak ele alan middleware.
/// Hata türüne göre uygun HTTP durum kodu döner ve hatayı Serilog ile loglar.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Response.Headers["X-Correlation-Id"].FirstOrDefault() ?? "N/A";

        // Exception türüne göre HTTP durum kodu belirle
        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Kaynak Bulunamadı"),
            ArgumentException => (HttpStatusCode.BadRequest, "Geçersiz İstek"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Yetkisiz Erişim"),
            _ => (HttpStatusCode.InternalServerError, "Sunucu Hatası")
        };

        // Hatayı CorrelationId ile birlikte logla
        _logger.LogError(exception,
            "İşlenmeyen hata. CorrelationId: {CorrelationId} | Hata: {Message}",
            correlationId, exception.Message);

        // Standart hata yanıtı oluştur
        var response = new
        {
            title,
            status = (int)statusCode,
            detail = exception.Message,
            correlationId,
            timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
