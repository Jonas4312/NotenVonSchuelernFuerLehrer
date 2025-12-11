using Microsoft.AspNetCore.Mvc;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[ApiController]
[Route("api/lehrer")]
public class LehrerController : ControllerBase
{
    [HttpGet]
    public IActionResult GetLehrer()
    {
        // Placeholder - Jonas bindet DB an
        var sample = new[] { new { Id = 1, Vorname = "Max", Nachname = "Mustermann" } };
        return Ok(sample);
    }
}
