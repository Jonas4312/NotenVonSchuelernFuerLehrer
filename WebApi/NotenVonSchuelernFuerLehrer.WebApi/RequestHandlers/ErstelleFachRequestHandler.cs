using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class ErstelleFachRequestHandler : BaseRequestHandler<ErstelleFachRequest, ErstelleFachResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public ErstelleFachRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<ErstelleFachResponse> HandleAsync(ErstelleFachRequest request)
    {
        var fach = new Fach
        {
            Id = Guid.NewGuid(),
            Bezeichnung = request.Bezeichnung,
            Kurzbezeichnung = request.Kurzbezeichnung,
            Lehrer = [],
            Noten = []
        };

        var entry = _context.Fach.Add(fach);
        await _context.SaveChangesAsync();

        return new ErstelleFachResponse
        {
            Fach = FachDto.Convert(entry.Entity)
        };
    }
}

public class ErstelleFachRequest : IRequest
{
    public required string Bezeichnung { get; init; }
    public required string Kurzbezeichnung { get; init; }
}

public class ErstelleFachResponse : IResponse
{
    public required FachDto Fach { get; init; }
}
