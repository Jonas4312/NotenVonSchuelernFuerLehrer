using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class AenderFachRequestHandler : BaseRequestHandler<AenderFachRequest, AenderFachResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly FachRepository _fachRepository;

    public AenderFachRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, FachRepository fachRepository)
    {
        _context = context;
        _fachRepository = fachRepository;
    }

    protected override async Task<AenderFachResponse> HandleAsync(AenderFachRequest request)
    {
        var fach = await _fachRepository.LadeFachAsync(request.FachId);
        
        fach.Bezeichnung = request.Bezeichnung;
        fach.Kurzbezeichnung = request.Kurzbezeichnung;
        
        await _context.SaveChangesAsync();

        return new AenderFachResponse
        {
            Fach = FachDto.Convert(fach)
        };
    }
}

public class AenderFachRequest : IRequest
{
    public required Guid FachId { get; init; }
    public required string Bezeichnung { get; init; }
    public required string Kurzbezeichnung { get; init; }
}

public class AenderFachResponse : IResponse
{
    public required FachDto Fach { get; init; }
}
