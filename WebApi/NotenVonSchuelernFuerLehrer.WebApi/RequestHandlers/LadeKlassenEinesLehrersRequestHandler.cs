using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeKlassenEinesLehrersRequestHandler : BaseRequestHandler<LadeKlassenEinesLehrersRequest, LadeKlassenEinesLehrersResponse>
{
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly LehrerRepository _lehrerRepository;

    public LadeKlassenEinesLehrersRequestHandler(LehrerAccessor lehrerAccessor, LehrerRepository lehrerRepository)
    {
        _lehrerAccessor = lehrerAccessor;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<LadeKlassenEinesLehrersResponse> HandleAsync(LadeKlassenEinesLehrersRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitFaecherUndKlassenAsync(jwtLehrer.Id);
        
        return new LadeKlassenEinesLehrersResponse
        {
            Klassen = lehrer.Faecher
                .SelectMany(f => f.Klassen)
                .DistinctBy(k => k.Id)
                .Select(KlasseDto.Convert)
                .ToList()
        };
    }
}

public class LadeKlassenEinesLehrersRequest : IRequest;

public class LadeKlassenEinesLehrersResponse : IResponse
{
    public required List<KlasseDto> Klassen { get; init; }
}