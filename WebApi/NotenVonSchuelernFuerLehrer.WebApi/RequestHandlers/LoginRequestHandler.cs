using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Exceptions;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LoginRequestHandler : BaseRequestHandler<LoginRequest, LoginResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly LehrerRepository _lehrerRepository;
    private readonly JwtService _jwtService;
    private readonly HashService _hashSerivce;

    public LoginRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, LehrerRepository lehrerRepository, JwtService jwtService, HashService hashSerivce)
    {
        _context = context;
        _lehrerRepository = lehrerRepository;
        _jwtService = jwtService;
        _hashSerivce = hashSerivce;
    }

    protected override async Task<LoginResponse> HandleAsync(LoginRequest request)
    {
        var lehrer = await _lehrerRepository.LadeLehrerMitKlassenAsync(request.Username);

        if (lehrer is not null && _hashSerivce.IsValidPassword(request.Password, lehrer.PasswortHash))
        {
            // Fächer separat laden
            var faecher = await _context.Lehrer
                .Where(l => l.Id == lehrer.Id)
                .SelectMany(l => l.Faecher)
                .ToListAsync();
            
            return new LoginResponse
            {
                Token = _jwtService.GenerateToken(new JwtLehrer
                {
                    Id = lehrer.Id,
                    Benutzername = lehrer.Benutzername,
                    Vorname = lehrer.Vorname,
                    Nachname = lehrer.Nachname
                }),
                Lehrer = LehrerDto.Convert(lehrer),
                Faecher = faecher.Select(FachDto.Convert).ToList(),
                Klassen = lehrer.Klassen.Select(KlasseDto.Convert).ToList()
            };
        }

        throw new ValidationException("Ungültiger Benutzername oder Passwort.");
    }
}

public class LoginRequest : IRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}

public class LoginResponse : IResponse
{
    public required string Token { get; init; }
    public required LehrerDto Lehrer { get; init; }
    public required List<FachDto> Faecher { get; init; }
    public required List<KlasseDto> Klassen { get; init; }
}