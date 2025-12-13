using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeLehrerRequestHandler : BaseRequestHandler<LadeLehrerRequest, LadeLehrerResponse>
{
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly LehrerRepository _lehrerRepository;

    public LadeLehrerRequestHandler(LehrerAccessor lehrerAccessor, LehrerRepository lehrerRepository)
    {
        _lehrerAccessor = lehrerAccessor;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<LadeLehrerResponse> HandleAsync(LadeLehrerRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitFaecherUndKlassenAsync(jwtLehrer.Id);
        return new LadeLehrerResponse
        {
            Lehrer = LehrerDto.Convert(lehrer),
            Faecher = lehrer.Faecher
                .Select(FachDto.Convert)
                .ToList(),
            Klassen = lehrer.Faecher
                .SelectMany(f => f.Klassen)
                .DistinctBy(k => k.Id)
                .Select(KlasseDto.Convert)
                .ToList()
        };
    }
}

public class LadeLehrerRequest : IRequest;

public class LadeLehrerResponse : IResponse
{
    public required LehrerDto Lehrer { get; init; }
    public required List<FachDto> Faecher { get; init; }
    public required List<KlasseDto> Klassen { get; init; }
}