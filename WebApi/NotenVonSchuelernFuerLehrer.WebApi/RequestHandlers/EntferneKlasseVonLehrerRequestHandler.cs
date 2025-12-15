using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class EntferneKlasseVonLehrerRequestHandler : BaseRequestHandler<EntferneKlasseVonLehrerRequest, EntferneKlasseVonLehrerResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public EntferneKlasseVonLehrerRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<EntferneKlasseVonLehrerResponse> HandleAsync(EntferneKlasseVonLehrerRequest request)
    {
        var klasse = await _context.Klasse
            .Include(k => k.Lehrer)
            .FirstAsync(k => k.Id == request.KlasseId);
        
        var lehrer = klasse.Lehrer.FirstOrDefault(l => l.Id == request.LehrerId);
        if (lehrer != null)
        {
            klasse.Lehrer.Remove(lehrer);
            await _context.SaveChangesAsync();
        }

        return new EntferneKlasseVonLehrerResponse();
    }
}

public class EntferneKlasseVonLehrerRequest : IRequest
{
    public required Guid KlasseId { get; init; }
    public required Guid LehrerId { get; init; }
}

public class EntferneKlasseVonLehrerResponse : IEmptyResponse;
