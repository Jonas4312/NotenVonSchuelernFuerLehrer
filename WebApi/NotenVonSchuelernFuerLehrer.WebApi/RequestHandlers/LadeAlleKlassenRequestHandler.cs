using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeAlleKlassenRequestHandler : BaseRequestHandler<LadeAlleKlassenRequest, LadeAlleKlassenResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public LadeAlleKlassenRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<LadeAlleKlassenResponse> HandleAsync(LadeAlleKlassenRequest request)
    {
        var klassen = await _context.Klasse
            .Include(k => k.Schueler)
            .ToListAsync();
        
        return new LadeAlleKlassenResponse
        {
            Klassen = klassen.Select(KlasseDto.Convert).ToList()
        };
    }
}

public class LadeAlleKlassenRequest : IRequest;

public class LadeAlleKlassenResponse : IResponse
{
    public required List<KlasseDto> Klassen { get; init; }
}
