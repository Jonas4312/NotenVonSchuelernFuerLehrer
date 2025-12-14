using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class FachController : ControllerBase
{
    private readonly RequestExecutor _requestExecutor;

    public FachController(RequestExecutor requestExecutor)
    {
        _requestExecutor = requestExecutor;
    }

    [HttpGet]
    public async Task<IActionResult> GetFaecher()
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeFaecherRequest());
    }

    [HttpGet("{fachId:guid}")]
    public async Task<IActionResult> GetFachById(Guid fachId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeFachRequest
        {
            FachId = fachId
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateFach(ErstelleFachRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateFach(AenderFachRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteFach(LoescheFachRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }

    [HttpPost("{fachId:guid}/klasse/{klasseId:guid}")]
    public async Task<IActionResult> WeiseFachKlasseZu(Guid fachId, Guid klasseId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new WeiseFachKlasseZuRequest
        {
            FachId = fachId,
            KlasseId = klasseId
        });
    }

    [HttpDelete("{fachId:guid}/klasse/{klasseId:guid}")]
    public async Task<IActionResult> EntferneFachVonKlasse(Guid fachId, Guid klasseId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new EntferneFachVonKlasseRequest
        {
            FachId = fachId,
            KlasseId = klasseId
        });
    }

    [HttpPost("{fachId:guid}/lehrer/{lehrerId:guid}")]
    public async Task<IActionResult> WeiseFachLehrerZu(Guid fachId, Guid lehrerId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new WeiseFachLehrerZuRequest
        {
            FachId = fachId,
            LehrerId = lehrerId
        });
    }

    [HttpDelete("{fachId:guid}/lehrer/{lehrerId:guid}")]
    public async Task<IActionResult> EntferneFachVonLehrer(Guid fachId, Guid lehrerId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new EntferneFachVonLehrerRequest
        {
            FachId = fachId,
            LehrerId = lehrerId
        });
    }
}
