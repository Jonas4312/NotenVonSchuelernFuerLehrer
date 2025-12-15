using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class ErstelleSchuelerRequestHandler : BaseRequestHandler<ErstelleSchuelerRequest, ErstelleSchuelerResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly KlasseRepository _klasseRepository;
    private readonly LehrerRepository _lehrerRepository;
    private readonly LehrerAccessor _lehrerAccessor;
    
    public ErstelleSchuelerRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, KlasseRepository klasseRepository, LehrerRepository lehrerRepository, LehrerAccessor lehrerAccessor)
    {
        _context = context;
        _klasseRepository = klasseRepository;
        _lehrerRepository = lehrerRepository;
        _lehrerAccessor = lehrerAccessor;
    }

    protected override async Task<ErstelleSchuelerResponse> HandleAsync(ErstelleSchuelerRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitKlassenAsync(jwtLehrer.Id);
        var klasse = await _klasseRepository.LadeKlasseAsync(request.KlasseId);
        
        lehrer.DarfKlasseVerwalten(klasse.Id);
        
        var schueler = new Schueler
        {
            Id = Guid.NewGuid(),
            KlasseId = klasse.Id,
            Vorname = request.Vorname,
            Nachname = request.Nachname,
            BildByteArray = request.BildByteArray,
            Klasse = klasse,
            Noten = []
        };

        klasse.Schueler.Add(schueler);
        var entry = _context.Schueler.Add(schueler);
        
        await _context.SaveChangesAsync();

        return new ErstelleSchuelerResponse
        {
            Schueler = SchuelerDto.Convert(entry.Entity)
        };
    }
}

public class ErstelleSchuelerRequest : IRequest
{
    public required Guid KlasseId { get; set; }
    public required string Vorname { get; init; }
    public required string Nachname { get; init; }
    public required byte[] BildByteArray { get; init; }
}

public class ErstelleSchuelerResponse : IResponse
{
    public required SchuelerDto Schueler { get; init; }
}