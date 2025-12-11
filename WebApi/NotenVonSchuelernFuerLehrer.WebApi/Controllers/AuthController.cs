using Microsoft.AspNetCore.Mvc;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.DTOs;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[ApiController]
[Route("login")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwt;
    private readonly LehrerRepository _lehrerRepository;
    private readonly HashService _hashSerivce;

    public AuthController(IJwtService jwt, LehrerRepository lehrerRepository, HashService hashSerivce)
    {
        _jwt = jwt;
        _lehrerRepository = lehrerRepository;
        _hashSerivce = hashSerivce;
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var lehrer = await _lehrerRepository.LadeLehrerAnBenutzernameAsync(request.Username);

        if(lehrer is not null && _hashSerivce.IsValidPassword(request.Password, lehrer.PasswortHash))
        {
            var token = _jwt.GenerateToken(request.Username);
            return Ok(new LoginResponse { Token = token });
        }

        return Unauthorized(new { message = "Falsche Login-Daten" });
    }
}
