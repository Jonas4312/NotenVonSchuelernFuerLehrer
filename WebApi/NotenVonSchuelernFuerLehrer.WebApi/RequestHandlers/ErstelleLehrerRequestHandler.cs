using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class ErstelleLehrerRequestHandler : BaseRequestHandler<ErstelleLehrerRequest, ErstelleLehrerResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly HashService _hashService;

    public ErstelleLehrerRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, HashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }

    protected override async Task<ErstelleLehrerResponse> HandleAsync(ErstelleLehrerRequest request)
    {
        var lehrer = new Lehrer
        {
            Id = Guid.NewGuid(),
            Vorname = request.Vorname,
            Nachname = request.Nachname,
            Benutzername = request.Benutzername,
            PasswortHash = _hashService.HashPassword(request.Passwort),
            BildByteArray = request.BildByteArray,
            Faecher = []
        };

        var entry = _context.Lehrer.Add(lehrer);
        await _context.SaveChangesAsync();

        return new ErstelleLehrerResponse
        {
            Lehrer = LehrerDto.Convert(entry.Entity)
        };
    }
}

public class ErstelleLehrerRequest : IRequest
{
    public required string Vorname { get; init; }
    public required string Nachname { get; init; }
    public required string Benutzername { get; init; }
    public required string Passwort { get; init; }
    public required byte[] BildByteArray { get; init; }
}

public class ErstelleLehrerResponse : IResponse
{
    public required LehrerDto Lehrer { get; init; }
}
