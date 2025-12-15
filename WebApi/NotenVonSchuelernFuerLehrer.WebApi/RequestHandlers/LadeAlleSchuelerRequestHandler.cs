using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeAlleSchuelerRequestHandler : BaseRequestHandler<LadeAlleSchuelerRequest, LadeAlleSchuelerResponse>
{
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly SchuelerRepository _schuelerRepository;
    private readonly LehrerRepository _lehrerRepository;

    public LadeAlleSchuelerRequestHandler(LehrerAccessor lehrerAccessor, SchuelerRepository schuelerRepository, LehrerRepository lehrerRepository)
    {
        _lehrerAccessor = lehrerAccessor;
        _schuelerRepository = schuelerRepository;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<LadeAlleSchuelerResponse> HandleAsync(LadeAlleSchuelerRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitKlassenAsync(jwtLehrer.Id);
        
        // Alle Klassen-IDs des Lehrers ermitteln (direkte Zuordnung)
        var erlaubteKlassenIds = lehrer.Klassen
            .Select(k => k.Id)
            .ToHashSet();
        
        var alleSchueler = await _schuelerRepository.LadeAlleSchuelerAsync();
        
        // Nur Schüler aus Klassen, die der Lehrer unterrichtet
        var filteredSchueler = alleSchueler
            .Where(s => erlaubteKlassenIds.Contains(s.KlasseId))
            .ToList();
        
        return new LadeAlleSchuelerResponse
        {
            Schueler = filteredSchueler.Select(SchuelerDto.Convert).ToList()
        };
    }
}

public class LadeAlleSchuelerRequest : IRequest
{
}

public class LadeAlleSchuelerResponse : IResponse
{
    public required List<SchuelerDto> Schueler { get; set; }
}
