using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LoescheKlasseRequestHandler : BaseRequestHandler<LoescheKlasseRequest, LoescheKlasseResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly KlasseRepository _klasseRepository;

    public LoescheKlasseRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, KlasseRepository klasseRepository)
    {
        _context = context;
        _klasseRepository = klasseRepository;
    }

    protected override async Task<LoescheKlasseResponse> HandleAsync(LoescheKlasseRequest request)
    {
        var klasse = await _klasseRepository.LadeKlasseAsync(request.KlasseId);
        
        klasse.IsDeleted = true;
        await _context.SaveChangesAsync();

        return new LoescheKlasseResponse();
    }
}

public class LoescheKlasseRequest : IRequest
{
    public required Guid KlasseId { get; init; }
}

public class LoescheKlasseResponse : IEmptyResponse;
