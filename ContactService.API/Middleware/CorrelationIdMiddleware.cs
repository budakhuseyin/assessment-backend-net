using Serilog.Context;

namespace ContactService.API.Middleware;

/// <summary>
/// Her HTTP isteğine benzersiz bir CorrelationId atar.
/// Bu ID hem response header'larına hem de Serilog log context'ine eklenir.
/// Böylece dağıtık sistemlerde tek bir isteği uçtan uca takip etmek mümkün olur.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // İstekte zaten bir CorrelationId var mı kontrol et, yoksa yeni üret
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        // Response header'ına ekle (client da görsün)
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        // Serilog log context'ine ekle (tüm loglarda görünsün)
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
