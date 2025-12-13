using Microsoft.AspNetCore.Mvc;
using NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly RequestExecutor _requestExecutor;

    public AuthController(RequestExecutor requestExecutor)
    {
        _requestExecutor = requestExecutor;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }
}