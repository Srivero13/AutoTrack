using AutoTrack.Api.Response;
using AutoTrack.Core.Exceptions;
using FluentValidation;

namespace AutoTrack.Api.Middleware;

public class ManejadorErroresMiddleware
{
    private readonly RequestDelegate _siguiente;

    public ManejadorErroresMiddleware(RequestDelegate siguiente)
    {
        _siguiente = siguiente;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (ValidationException ex)
        {
            var errores = ex.Errors.Select(x => x.PropertyName + ": " + x.ErrorMessage).ToList();
            await ResponderAsync(contexto, 400, "Error de validación", errores);
        }
        catch (RecursoNoEncontradoException ex)
        {
            await ResponderAsync(contexto, 404, ex.Message, new List<string> { ex.Message });
        }
        catch (ReglaNegocioException ex)
        {
            await ResponderAsync(contexto, 409, ex.Message, new List<string> { ex.Message });
        }
        catch (Exception)
        {
            await ResponderAsync(contexto, 500, "Error interno del servidor", new List<string>());
        }
    }

    private static async Task ResponderAsync(HttpContext contexto, int estado, string mensaje, List<string> errores)
    {
        contexto.Response.StatusCode = estado;
        await contexto.Response.WriteAsJsonAsync(ApiResponse<object>.Fallo(mensaje, errores));
    }
}
