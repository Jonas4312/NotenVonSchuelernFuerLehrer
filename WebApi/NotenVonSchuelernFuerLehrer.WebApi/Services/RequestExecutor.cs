using Microsoft.AspNetCore.Mvc;
using NotenVonSchuelernFuerLehrer.Domain.Service.Exceptions;
using NotenVonSchuelernFuerLehrer.WebApi.Exceptions;
using NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

namespace NotenVonSchuelernFuerLehrer.WebApi.Services;

public class RequestExecutor
{
    private readonly IEnumerable<IRequestHandler> _requestHandlers;

    public RequestExecutor(IEnumerable<IRequestHandler> requestHandlers)
    {
        _requestHandlers = requestHandlers;
    }

    public async Task<IActionResult> ExecuteRequestAsync(IRequest request)
    {
        try
        {
            var handler = _requestHandlers.FirstOrDefault(h => h.CanHandle(request));

            if (handler is null)
            {
                throw new InvalidOperationException($"No handler found for request type {request.GetType().Name}");
            }

            var response = await handler.HandleInternalAsync(request);
            
            return new OkObjectResult(new ApiResponse
            {
                Success = true,
                Data = response is IEmptyResponse ? null : response
            });
        }
        catch (LehrerNichtGefundenException ex)
        {
            // Lehrer existiert nicht mehr in DB (z.B. nach DB-Reset) → 401 zurückgeben
            return new UnauthorizedObjectResult(new ApiResponse
            {
                Success = false,
                Errors = [ex.Message]
            });
        }
        catch (AuthenticationException authException)
        {
            return new UnauthorizedObjectResult(new ApiResponse
            {
                Success = false,
                Errors = [authException.Message]
            });
        }
        catch (ValidationException validationException)
        {
            return new BadRequestObjectResult(new ApiResponse
            {
                Success = false,
                ValidationErrors = validationException.ValidationErrors
            });
        }
    }
}

public class ApiResponse
{
    public required bool Success { get; init; }
    public List<string> ValidationErrors { get; init; } = [];
    public List<string> Errors { get; init; } = [];
    public object? Data { get; init; }
}