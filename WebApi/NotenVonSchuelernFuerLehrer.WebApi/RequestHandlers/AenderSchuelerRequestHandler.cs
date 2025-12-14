using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class AenderSchuelerRequestHandler : BaseRequestHandler<AenderSchuelerRequest, AenderSchuelerResponse>
{
    private readonly SchuelerRepository _schuelerRepository;
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly LehrerRepository _lehrerRepository;
    private readonly LehrerAccessor _lehrerAccessor;

    public AenderSchuelerRequestHandler(SchuelerRepository schuelerRepository, NotenVonSchuelernFuerLehrerDbContext context, LehrerRepository lehrerRepository, LehrerAccessor lehrerAccessor)
    {
        _schuelerRepository = schuelerRepository;
        _context = context;
        _lehrerRepository = lehrerRepository;
        _lehrerAccessor = lehrerAccessor;
    }

    protected override async Task<AenderSchuelerResponse> HandleAsync(AenderSchuelerRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitFaecherUndKlassenAsync(jwtLehrer.Id);
        var schueler = await _schuelerRepository.LadeSchuelerAsync(request.Id);
        
        lehrer.DarfKlasseVerwalten(schueler.KlasseId);
        
        schueler.Vorname = request.Vorname;
        schueler.Nachname = request.Nachname;
        schueler.BildByteArray = request.BildByteArray;
        
        await _context.SaveChangesAsync();
        
        return new AenderSchuelerResponse
        {
            Schueler = SchuelerDto.Convert(schueler)
        };
    }
}

public class AenderSchuelerRequest : IRequest
{
    public required Guid Id { get; init; }
    public required string Vorname { get; init; }
    public required string Nachname { get; init; }
    public required byte[] BildByteArray { get; init; }
}

public class AenderSchuelerResponse : IResponse
{
    public required SchuelerDto Schueler { get; init; }   
}