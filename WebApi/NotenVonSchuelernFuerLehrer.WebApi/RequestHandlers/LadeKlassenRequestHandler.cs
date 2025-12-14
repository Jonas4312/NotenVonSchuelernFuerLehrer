using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeKlassenRequestHandler : BaseRequestHandler<LadeKlassenRequest, LadeKlassenResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public LadeKlassenRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<LadeKlassenResponse> HandleAsync(LadeKlassenRequest request)
    {
        var klassen = await _context.Klasse.ToListAsync();
        
        return new LadeKlassenResponse
        {
            Klassen = klassen.Select(KlasseDto.Convert).ToList()
        };
    }
}

public class LadeKlassenRequest : IRequest;

public class LadeKlassenResponse : IResponse
{
    public required List<KlasseDto> Klassen { get; init; }
}
