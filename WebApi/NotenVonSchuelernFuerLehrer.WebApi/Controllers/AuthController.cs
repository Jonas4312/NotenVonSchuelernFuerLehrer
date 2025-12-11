using Microsoft.AspNetCore.Mvc;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.DTOs;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.Controllers;

[ApiController]
[Route("login")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly LehrerRepository _lehrerRepository;
    private readonly HashService _hashSerivce;

    public AuthController(JwtService jwtService, LehrerRepository lehrerRepository, HashService hashSerivce)
    {
        _jwtService = jwtService;
        _lehrerRepository = lehrerRepository;
        _hashSerivce = hashSerivce;
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var lehrer = await _lehrerRepository.LadeLehrerAnBenutzernameAsync(request.Username);

        if(lehrer is not null && _hashSerivce.IsValidPassword(request.Password, lehrer.PasswortHash))
        {
            var token = _jwtService.GenerateToken(lehrer);
            return Ok(new LoginResponse { Token = token });
        }

        return Unauthorized(new { message = "Falsche Login-Daten" });
    }
}
