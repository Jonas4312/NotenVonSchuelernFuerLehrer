using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LoescheSchuelerRequestHandler : BaseRequestHandler<LoescheSchuelerRequest, LoescheSchuelerResponse>
{
    private readonly SchuelerRepository _schuelerRepository;
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly LehrerRepository _lehrerRepository;
    private readonly LehrerAccessor _lehrerAccessor;

    public LoescheSchuelerRequestHandler(SchuelerRepository schuelerRepository, NotenVonSchuelernFuerLehrerDbContext context, LehrerRepository lehrerRepository, LehrerAccessor lehrerAccessor)
    {
        _schuelerRepository = schuelerRepository;
        _context = context;
        _lehrerRepository = lehrerRepository;
        _lehrerAccessor = lehrerAccessor;
    }

    protected override async Task<LoescheSchuelerResponse> HandleAsync(LoescheSchuelerRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitKlassenAsync(jwtLehrer.Id);
        var schueler = await _schuelerRepository.LadeSchuelerAsync(request.SchuelerId);
        
        lehrer.DarfKlasseVerwalten(schueler.KlasseId);
        
        schueler.IsDeleted = true;
        await _context.SaveChangesAsync();
        
        return new LoescheSchuelerResponse();
    }
}

public class LoescheSchuelerRequest : IRequest
{
    public required Guid SchuelerId { get; init; }
}

public class LoescheSchuelerResponse : IEmptyResponse;