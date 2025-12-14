using Microsoft.AspNetCore.Mvc;
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