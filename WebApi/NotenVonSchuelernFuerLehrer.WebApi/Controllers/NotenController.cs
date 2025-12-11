using Microsoft.AspNetCore.Mvc;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[ApiController]
[Route("api/data/note")]
public class NotenController : ControllerBase
{
    [HttpGet]
    public IActionResult GetNoten()
    {
        var sample = new[] { new { Id = 1, SchuelerId = 1, Wert = 1.3, Fach = "Mathe" } };
        return Ok(sample);
    }

    [HttpGet("{noteId}")]
    public IActionResult GetNoteById(int noteId)
    {
        return Ok(new { Id = noteId, SchuelerId = 1, Wert = 2.0, Fach = "Deutsch" });
    }
}
