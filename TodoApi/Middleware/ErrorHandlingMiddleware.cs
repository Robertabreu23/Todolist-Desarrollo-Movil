using System.Net;
using System.Text.Json;

namespace TodoApi.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en {Path}", context.Request.Path);
            await WriteProblemAsync(context, ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            ArgumentException => ((int)HttpStatusCode.BadRequest, "Solicitud inválida"),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Recurso no encontrado"),
            InvalidOperationException => ((int)HttpStatusCode.Conflict, "Operación inválida"),
            _ => ((int)HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var payload = new
        {
            type = $"https://httpstatuses.io/{status}",
            title,
            status,
            detail = _env.IsDevelopment() ? ex.Message : null,
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }
}
