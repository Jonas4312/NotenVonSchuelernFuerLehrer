using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[Authorize]
[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]")]
public class NoteController : ControllerBase
{
    private readonly RequestExecutor _requestExecutor;

    public NoteController(RequestExecutor requestExecutor)
    {
        _requestExecutor = requestExecutor;
    }

    [HttpGet("{noteId:guid}")]
    public async Task<IActionResult> GetNote(Guid noteId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeNoteEinesSchuelersRequest
        {
            NoteId = noteId
        });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetNoten([FromQuery] Guid schuelerId)
    {
        return await _requestExecutor.ExecuteRequestAsync(new LadeNotenEinesSchuelersRequest
        {
            SchuelerId = schuelerId
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] ErstelleNoteRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateNote([FromBody] AenderNoteRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteNote([FromBody] LoescheNoteRequest request)
    {
        return await _requestExecutor.ExecuteRequestAsync(request);
    }
}
