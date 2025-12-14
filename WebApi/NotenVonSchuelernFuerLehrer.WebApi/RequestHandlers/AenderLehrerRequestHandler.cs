using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class AenderLehrerRequestHandler : BaseRequestHandler<AenderLehrerRequest, AenderLehrerResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly LehrerRepository _lehrerRepository;
    private readonly HashService _hashService;

    public AenderLehrerRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, LehrerRepository lehrerRepository, HashService hashService)
    {
        _context = context;
        _lehrerRepository = lehrerRepository;
        _hashService = hashService;
    }

    protected override async Task<AenderLehrerResponse> HandleAsync(AenderLehrerRequest request)
    {
        var lehrer = await _lehrerRepository.LadeLehrerAsync(request.LehrerId);
        
        lehrer.Vorname = request.Vorname;
        lehrer.Nachname = request.Nachname;
        lehrer.Benutzername = request.Benutzername;
        lehrer.BildByteArray = request.BildByteArray;
        
        if (!string.IsNullOrEmpty(request.Passwort))
        {
            lehrer.PasswortHash = _hashService.HashPassword(request.Passwort);
        }
        
        await _context.SaveChangesAsync();

        return new AenderLehrerResponse
        {
            Lehrer = LehrerDto.Convert(lehrer)
        };
    }
}

public class AenderLehrerRequest : IRequest
{
    public required Guid LehrerId { get; init; }
    public required string Vorname { get; init; }
    public required string Nachname { get; init; }
    public required string Benutzername { get; init; }
    public string? Passwort { get; init; }
    public required byte[] BildByteArray { get; init; }
}

public class AenderLehrerResponse : IResponse
{
    public required LehrerDto Lehrer { get; init; }
}
