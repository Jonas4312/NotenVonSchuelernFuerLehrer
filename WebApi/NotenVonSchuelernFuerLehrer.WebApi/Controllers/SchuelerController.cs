using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class SchuelerController : ControllerBase
{
    private readonly RequestExecutor _requestExecutor;

    public SchuelerController(RequestExecutor requestExecutor)
    {
        _requestExecutor = requestExecutor;
    }

    [HttpGet("{schuelerId:guid}")]
    public async Task<IActionResult> GetSchuelerById(Guid schuelerId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeSchuelerRequest
        {
            SchuelerId = schuelerId
        });
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteSchueler(LoescheSchuelerRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchueler(ErstelleSchuelerRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSchueler(AenderSchuelerRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }
}
