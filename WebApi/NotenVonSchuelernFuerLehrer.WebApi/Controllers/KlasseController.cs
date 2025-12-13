using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class KlasseController : ControllerBase
{
    private readonly RequestExecutor _requestExecutor;

    public KlasseController(RequestExecutor requestExecutor)
    {
        _requestExecutor = requestExecutor;
    }

    [HttpGet]
    public async Task<IActionResult> GetKlassen()
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeKlassenEinesLehrersRequest());
    }

    [HttpGet("{klasseId:guid}")]
    public async Task<IActionResult> GetKlasseById(Guid klasseId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeSchuelerEinerKlasseRequest
        {
            KlasseId = klasseId
        });
    }
}
