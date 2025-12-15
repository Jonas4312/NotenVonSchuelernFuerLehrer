using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeSchuelerRequestHandler : BaseRequestHandler<LadeSchuelerRequest, LadeSchuelerResponse>
{
    private readonly SchuelerRepository _schuelerRepository;
    private readonly LehrerRepository _lehrerRepository;
    private readonly LehrerAccessor _lehrerAccessor;

    public LadeSchuelerRequestHandler(SchuelerRepository schuelerRepository, LehrerRepository lehrerRepository, LehrerAccessor lehrerAccessor)
    {
        _schuelerRepository = schuelerRepository;
        _lehrerRepository = lehrerRepository;
        _lehrerAccessor = lehrerAccessor;
    }

    protected override async Task<LadeSchuelerResponse> HandleAsync(LadeSchuelerRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitKlassenAsync(jwtLehrer.Id);
        var schueler = await _schuelerRepository.LadeSchuelerAsync(request.SchuelerId);
        
        lehrer.DarfKlasseVerwalten(schueler.KlasseId);
        
        return new LadeSchuelerResponse
        {
            Schueler = SchuelerDto.Convert(schueler)
        };
    }
}

public class LadeSchuelerRequest : IRequest
{
    public required Guid SchuelerId { get; init; }
}

public class LadeSchuelerResponse : IResponse
{
    public required SchuelerDto Schueler { get; init; }
}