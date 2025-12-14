using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class WeiseFachLehrerZuRequestHandler : BaseRequestHandler<WeiseFachLehrerZuRequest, WeiseFachLehrerZuResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly LehrerRepository _lehrerRepository;

    public WeiseFachLehrerZuRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, LehrerRepository lehrerRepository)
    {
        _context = context;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<WeiseFachLehrerZuResponse> HandleAsync(WeiseFachLehrerZuRequest request)
    {
        var fach = await _context.Fach
            .Include(f => f.Lehrer)
            .FirstAsync(f => f.Id == request.FachId);
        var lehrer = await _lehrerRepository.LadeLehrerAsync(request.LehrerId);
        
        if (!fach.Lehrer.Any(l => l.Id == lehrer.Id))
        {
            fach.Lehrer.Add(lehrer);
            await _context.SaveChangesAsync();
        }

        return new WeiseFachLehrerZuResponse();
    }
}

public class WeiseFachLehrerZuRequest : IRequest
{
    public required Guid FachId { get; init; }
    public required Guid LehrerId { get; init; }
}

public class WeiseFachLehrerZuResponse : IEmptyResponse;
