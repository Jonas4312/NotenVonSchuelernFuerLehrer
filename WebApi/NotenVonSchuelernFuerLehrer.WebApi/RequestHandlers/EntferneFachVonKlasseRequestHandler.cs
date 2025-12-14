using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class EntferneFachVonKlasseRequestHandler : BaseRequestHandler<EntferneFachVonKlasseRequest, EntferneFachVonKlasseResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public EntferneFachVonKlasseRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<EntferneFachVonKlasseResponse> HandleAsync(EntferneFachVonKlasseRequest request)
    {
        var fach = await _context.Fach
            .Include(f => f.Klassen)
            .FirstAsync(f => f.Id == request.FachId);
        
        var klasse = fach.Klassen.FirstOrDefault(k => k.Id == request.KlasseId);
        if (klasse is not null)
        {
            fach.Klassen.Remove(klasse);
            await _context.SaveChangesAsync();
        }

        return new EntferneFachVonKlasseResponse();
    }
}

public class EntferneFachVonKlasseRequest : IRequest
{
    public required Guid FachId { get; init; }
    public required Guid KlasseId { get; init; }
}

public class EntferneFachVonKlasseResponse : IEmptyResponse;
