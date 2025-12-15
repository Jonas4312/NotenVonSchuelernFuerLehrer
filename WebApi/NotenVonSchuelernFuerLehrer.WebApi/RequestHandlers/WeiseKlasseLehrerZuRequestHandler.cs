using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class WeiseKlasseLehrerZuRequestHandler : BaseRequestHandler<WeiseKlasseLehrerZuRequest, WeiseKlasseLehrerZuResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly KlasseRepository _klasseRepository;
    private readonly LehrerRepository _lehrerRepository;

    public WeiseKlasseLehrerZuRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, KlasseRepository klasseRepository, LehrerRepository lehrerRepository)
    {
        _context = context;
        _klasseRepository = klasseRepository;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<WeiseKlasseLehrerZuResponse> HandleAsync(WeiseKlasseLehrerZuRequest request)
    {
        var klasse = await _context.Klasse
            .Include(k => k.Lehrer)
            .FirstAsync(k => k.Id == request.KlasseId);
        var lehrer = await _lehrerRepository.LadeLehrerAsync(request.LehrerId);
        
        if (!klasse.Lehrer.Any(l => l.Id == lehrer.Id))
        {
            klasse.Lehrer.Add(lehrer);
            await _context.SaveChangesAsync();
        }

        return new WeiseKlasseLehrerZuResponse();
    }
}

public class WeiseKlasseLehrerZuRequest : IRequest
{
    public required Guid KlasseId { get; init; }
    public required Guid LehrerId { get; init; }
}

public class WeiseKlasseLehrerZuResponse : IEmptyResponse;
