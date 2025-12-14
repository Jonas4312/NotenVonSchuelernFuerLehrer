using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class AenderKlasseRequestHandler : BaseRequestHandler<AenderKlasseRequest, AenderKlasseResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly KlasseRepository _klasseRepository;

    public AenderKlasseRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, KlasseRepository klasseRepository)
    {
        _context = context;
        _klasseRepository = klasseRepository;
    }

    protected override async Task<AenderKlasseResponse> HandleAsync(AenderKlasseRequest request)
    {
        var klasse = await _klasseRepository.LadeKlasseAsync(request.KlasseId);
        
        klasse.Bezeichnung = request.Bezeichnung;
        klasse.Kurzbezeichnung = request.Kurzbezeichnung;
        
        await _context.SaveChangesAsync();

        return new AenderKlasseResponse
        {
            Klasse = KlasseDto.Convert(klasse)
        };
    }
}

public class AenderKlasseRequest : IRequest
{
    public required Guid KlasseId { get; init; }
    public required string Bezeichnung { get; init; }
    public required string Kurzbezeichnung { get; init; }
}

public class AenderKlasseResponse : IResponse
{
    public required KlasseDto Klasse { get; init; }
}
