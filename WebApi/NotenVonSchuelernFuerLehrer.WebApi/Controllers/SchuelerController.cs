using Microsoft.AspNetCore.Mvc;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[ApiController]
[Route("api/data/schueler")]
public class SchuelerController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllSchueler()
    {
        var sample = new[] { new { Id = 1, Vorname = "Anna", Nachname = "Schmidt", KlasseId = 1 } };
        return Ok(sample);
    }

    [HttpGet("{schuelerId}")]
    public IActionResult GetSchuelerById(int schuelerId)
    {
        return Ok(new { Id = schuelerId, Vorname = "Anna", Nachname = "Schmidt", KlasseId = 1 });
    }
}
