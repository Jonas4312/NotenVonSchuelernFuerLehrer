using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeFachRequestHandler : BaseRequestHandler<LadeFachRequest, LadeFachResponse>
{
    private readonly FachRepository _fachRepository;

    public LadeFachRequestHandler(FachRepository fachRepository)
    {
        _fachRepository = fachRepository;
    }

    protected override async Task<LadeFachResponse> HandleAsync(LadeFachRequest request)
    {
        var fach = await _fachRepository.LadeFachAsync(request.FachId);
        
        return new LadeFachResponse
        {
            Fach = FachDto.Convert(fach)
        };
    }
}

public class LadeFachRequest : IRequest
{
    public required Guid FachId { get; init; }
}

public class LadeFachResponse : IResponse
{
    public required FachDto Fach { get; init; }
}
