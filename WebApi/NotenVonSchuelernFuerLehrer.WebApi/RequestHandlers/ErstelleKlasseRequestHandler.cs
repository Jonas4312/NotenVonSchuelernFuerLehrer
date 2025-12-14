using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class ErstelleKlasseRequestHandler : BaseRequestHandler<ErstelleKlasseRequest, ErstelleKlasseResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public ErstelleKlasseRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<ErstelleKlasseResponse> HandleAsync(ErstelleKlasseRequest request)
    {
        var klasse = new Klasse
        {
            Id = Guid.NewGuid(),
            Bezeichnung = request.Bezeichnung,
            Kurzbezeichnung = request.Kurzbezeichnung,
            Schueler = [],
            Faecher = []
        };

        var entry = _context.Klasse.Add(klasse);
        await _context.SaveChangesAsync();

        return new ErstelleKlasseResponse
        {
            Klasse = KlasseDto.Convert(entry.Entity)
        };
    }
}

public class ErstelleKlasseRequest : IRequest
{
    public required string Bezeichnung { get; init; }
    public required string Kurzbezeichnung { get; init; }
}

public class ErstelleKlasseResponse : IResponse
{
    public required KlasseDto Klasse { get; init; }
}
