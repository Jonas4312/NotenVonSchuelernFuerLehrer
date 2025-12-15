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
    public async Task<IActionResult> GetAlleLehrer()
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeAlleLehrerRequest());
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetAktuellerLehrer()
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeLehrerRequest());
    }

    [HttpGet("{lehrerId:guid}")]
    public async Task<IActionResult> GetLehrerById(Guid lehrerId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeEinenLehrerRequest { LehrerId = lehrerId });
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
