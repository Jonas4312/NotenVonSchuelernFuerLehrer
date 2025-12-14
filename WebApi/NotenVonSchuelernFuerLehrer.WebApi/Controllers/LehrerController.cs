using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class LehrerController : ControllerBase
{
    private readonly RequestExecutor _requestExecutor;

    public LehrerController(RequestExecutor requestExecutor)
    {
        _requestExecutor = requestExecutor;
    }

    [HttpGet]
    public async Task<IActionResult> GetLehrer()
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeLehrerRequest());
    }
}
