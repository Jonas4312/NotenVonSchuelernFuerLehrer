using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class WeiseFachKlasseZuRequestHandler : BaseRequestHandler<WeiseFachKlasseZuRequest, WeiseFachKlasseZuResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly FachRepository _fachRepository;
    private readonly KlasseRepository _klasseRepository;

    public WeiseFachKlasseZuRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, FachRepository fachRepository, KlasseRepository klasseRepository)
    {
        _context = context;
        _fachRepository = fachRepository;
        _klasseRepository = klasseRepository;
    }

    protected override async Task<WeiseFachKlasseZuResponse> HandleAsync(WeiseFachKlasseZuRequest request)
    {
        var fach = await _context.Fach
            .Include(f => f.Klassen)
            .FirstAsync(f => f.Id == request.FachId);
        var klasse = await _klasseRepository.LadeKlasseAsync(request.KlasseId);
        
        if (!fach.Klassen.Any(k => k.Id == klasse.Id))
        {
            fach.Klassen.Add(klasse);
            await _context.SaveChangesAsync();
        }

        return new WeiseFachKlasseZuResponse();
    }
}

public class WeiseFachKlasseZuRequest : IRequest
{
    public required Guid FachId { get; init; }
    public required Guid KlasseId { get; init; }
}

public class WeiseFachKlasseZuResponse : IEmptyResponse;
