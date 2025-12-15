using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

// Handler für GET /lehrer/me - eingeloggten Lehrer laden
public class LadeLehrerRequestHandler : BaseRequestHandler<LadeLehrerRequest, LadeLehrerResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly LehrerRepository _lehrerRepository;

    public LadeLehrerRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, LehrerAccessor lehrerAccessor, LehrerRepository lehrerRepository)
    {
        _context = context;
        _lehrerAccessor = lehrerAccessor;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<LadeLehrerResponse> HandleAsync(LadeLehrerRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitKlassenAsync(jwtLehrer.Id);
        
        // Fächer separat laden
        var faecher = await _context.Lehrer
            .Where(l => l.Id == lehrer.Id)
            .SelectMany(l => l.Faecher)
            .ToListAsync();
        
        return new LadeLehrerResponse
        {
            Lehrer = LehrerDto.Convert(lehrer),
            Faecher = faecher.Select(FachDto.Convert).ToList(),
            Klassen = lehrer.Klassen.Select(KlasseDto.Convert).ToList()
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

// Handler für GET /lehrer - alle Lehrer laden
public class LadeAlleLehrerRequestHandler : BaseRequestHandler<LadeAlleLehrerRequest, LadeAlleLehrerResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public LadeAlleLehrerRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<LadeAlleLehrerResponse> HandleAsync(LadeAlleLehrerRequest request)
    {
        var lehrer = await _context.Lehrer
            .Include(l => l.Faecher)
            .Include(l => l.Klassen)
            .ToListAsync();
        
        return new LadeAlleLehrerResponse
        {
            Lehrer = lehrer.Select(LehrerDto.Convert).ToList()
        };
    }
}

public class LadeAlleLehrerRequest : IRequest;

public class LadeAlleLehrerResponse : IResponse
{
    public required List<LehrerDto> Lehrer { get; init; }
}

// Handler für GET /lehrer/{id} - einzelnen Lehrer laden
public class LadeEinenLehrerRequestHandler : BaseRequestHandler<LadeEinenLehrerRequest, LadeEinenLehrerResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public LadeEinenLehrerRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<LadeEinenLehrerResponse> HandleAsync(LadeEinenLehrerRequest request)
    {
        var lehrer = await _context.Lehrer
            .Include(l => l.Faecher)
            .Include(l => l.Klassen)
            .FirstOrDefaultAsync(l => l.Id == request.LehrerId);
        
        if (lehrer == null)
        {
            throw new InvalidOperationException($"Lehrer mit ID {request.LehrerId} nicht gefunden");
        }
        
        return new LadeEinenLehrerResponse
        {
            Lehrer = LehrerDto.Convert(lehrer),
            Faecher = lehrer.Faecher.Select(FachDto.Convert).ToList(),
            Klassen = lehrer.Klassen.Select(KlasseDto.Convert).ToList()
        };
    }
}

public class LadeEinenLehrerRequest : IRequest
{
    public required Guid LehrerId { get; init; }
}

public class LadeEinenLehrerResponse : IResponse
{
    public required LehrerDto Lehrer { get; init; }
    public required List<FachDto> Faecher { get; init; }
    public required List<KlasseDto> Klassen { get; init; }
}