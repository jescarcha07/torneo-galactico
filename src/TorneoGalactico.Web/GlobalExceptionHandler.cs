using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TorneoGalactico.Application.Common;
using TorneoGalactico.Domain;

namespace TorneoGalactico.Web;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
            BadHttpRequestException bad => (bad.StatusCode, "Solicitud inválida"),
            NotFoundException => (StatusCodes.Status404NotFound, "No encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflicto"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno")
        };

        if (status >= 500)
            logger.LogError(exception, "Error no controlado");

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            // Nunca exponemos detalles técnicos de un 500 al cliente
            Detail = status >= 500 ? "Ocurrió un error inesperado." : exception.Message
        }, ct);

        return true;
    }
}