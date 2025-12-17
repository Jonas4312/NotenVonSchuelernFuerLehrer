using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LoescheFachRequestHandler : BaseRequestHandler<LoescheFachRequest, LoescheFachResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly FachRepository _fachRepository;

    public LoescheFachRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, FachRepository fachRepository)
    {
        _context = context;
        _fachRepository = fachRepository;
    }

    protected override async Task<LoescheFachResponse> HandleAsync(LoescheFachRequest request)
    {
        var fach = await _fachRepository.LadeFachAsync(request.FachId);
        
        fach.IsDeleted = true;
        await _context.SaveChangesAsync();

        return new LoescheFachResponse();
    }
}

public class LoescheFachRequest : IRequest
{
    public required Guid FachId { get; init; }
}

public class LoescheFachResponse : IEmptyResponse;
