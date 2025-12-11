using Microsoft.AspNetCore.Mvc;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[ApiController]
[Route("api/data/klassen")]
public class KlassenController : ControllerBase
{
    [HttpGet]
    public IActionResult GetKlassen()
    {
        var sample = new[] { new { Id = 1, Bezeichnung = "10A" } };
        return Ok(sample);
    }

    [HttpGet("/api/data/klasse/{klasseId}")]
    public IActionResult GetKlasseById(int klasseId)
    {
        return Ok(new { Id = klasseId, Bezeichnung = $"Klasse {klasseId}" });
    }
}
