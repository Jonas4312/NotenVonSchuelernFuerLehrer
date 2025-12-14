using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Exceptions;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LoginRequestHandler : BaseRequestHandler<LoginRequest, LoginResponse>
{
    private readonly LehrerRepository _lehrerRepository;
    private readonly JwtService _jwtService;
    private readonly HashService _hashSerivce;

    public LoginRequestHandler(LehrerRepository lehrerRepository, JwtService jwtService, HashService hashSerivce)
    {
        _lehrerRepository = lehrerRepository;
        _jwtService = jwtService;
        _hashSerivce = hashSerivce;
    }

    protected override async Task<LoginResponse> HandleAsync(LoginRequest request)
    {
        var lehrer = await _lehrerRepository.LadeLehrerMitFaecherUndKlassenAsync(request.Username);

        if (lehrer is not null && _hashSerivce.IsValidPassword(request.Password, lehrer.PasswortHash))
        {
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
                Faecher = lehrer.Faecher
                    .Select(FachDto.Convert)
                    .ToList(),
                Klassen = lehrer.Faecher
                    .SelectMany(f => f.Klassen)
                    .DistinctBy(k => k.Id)
                    .Select(KlasseDto.Convert)
                    .ToList()
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