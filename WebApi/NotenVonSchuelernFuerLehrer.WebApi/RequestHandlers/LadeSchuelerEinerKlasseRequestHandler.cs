using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeSchuelerEinerKlasseRequestHandler : BaseRequestHandler<LadeSchuelerEinerKlasseRequest, LadeSchuelerEinerKlasseResponse>
{
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly SchuelerRepository _schuelerRepository;
    private readonly LehrerRepository _lehrerRepository;

    public LadeSchuelerEinerKlasseRequestHandler(LehrerAccessor lehrerAccessor, SchuelerRepository schuelerRepository, LehrerRepository lehrerRepository)
    {
        _lehrerAccessor = lehrerAccessor;
        _schuelerRepository = schuelerRepository;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<LadeSchuelerEinerKlasseResponse> HandleAsync(LadeSchuelerEinerKlasseRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitKlassenAsync(jwtLehrer.Id);
        
        lehrer.DarfKlasseVerwalten(request.KlasseId);
        
        var schueler = await _schuelerRepository.LadeAlleSchuelerEinerKlasseAsync(request.KlasseId);
        return new LadeSchuelerEinerKlasseResponse
        {
            Schueler = schueler.Select(SchuelerDto.Convert).ToList()
        };
    }
}

public class LadeSchuelerEinerKlasseRequest : IRequest
{
    public required Guid KlasseId { get; init; }
}

public class LadeSchuelerEinerKlasseResponse : IResponse
{
    public required List<SchuelerDto> Schueler { get; set; }
}