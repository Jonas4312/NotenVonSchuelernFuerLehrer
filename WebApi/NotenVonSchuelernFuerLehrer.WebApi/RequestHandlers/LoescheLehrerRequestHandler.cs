using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LoescheLehrerRequestHandler : BaseRequestHandler<LoescheLehrerRequest, LoescheLehrerResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly LehrerRepository _lehrerRepository;

    public LoescheLehrerRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, LehrerRepository lehrerRepository)
    {
        _context = context;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<LoescheLehrerResponse> HandleAsync(LoescheLehrerRequest request)
    {
        var lehrer = await _lehrerRepository.LadeLehrerAsync(request.LehrerId);
        
        lehrer.IsDeleted = true;
        // Benutzername bleibt unverändert - Unique-Constraint wurde entfernt.
        // Die Prüfung auf Eindeutigkeit erfolgt programmatisch nur für aktive Lehrer (IsDeleted = false).
        await _context.SaveChangesAsync();

        return new LoescheLehrerResponse();
    }
}

public class LoescheLehrerRequest : IRequest
{
    public required Guid LehrerId { get; init; }
}

public class LoescheLehrerResponse : IEmptyResponse;
