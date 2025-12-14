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

    [HttpPost]
    public async Task<IActionResult> CreateLehrer(ErstelleLehrerRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateLehrer(AenderLehrerRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteLehrer(LoescheLehrerRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }
}
