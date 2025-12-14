using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class EntferneFachVonLehrerRequestHandler : BaseRequestHandler<EntferneFachVonLehrerRequest, EntferneFachVonLehrerResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public EntferneFachVonLehrerRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<EntferneFachVonLehrerResponse> HandleAsync(EntferneFachVonLehrerRequest request)
    {
        var fach = await _context.Fach
            .Include(f => f.Lehrer)
            .FirstAsync(f => f.Id == request.FachId);
        
        var lehrer = fach.Lehrer.FirstOrDefault(l => l.Id == request.LehrerId);
        if (lehrer is not null)
        {
            fach.Lehrer.Remove(lehrer);
            await _context.SaveChangesAsync();
        }

        return new EntferneFachVonLehrerResponse();
    }
}

public class EntferneFachVonLehrerRequest : IRequest
{
    public required Guid FachId { get; init; }
    public required Guid LehrerId { get; init; }
}

public class EntferneFachVonLehrerResponse : IEmptyResponse;
