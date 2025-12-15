using Microsoft.Extensions.Options;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Configuration;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Exceptions;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class ErstelleNoteRequestHandler : BaseRequestHandler<ErstelleNoteRequest, ErstelleNoteResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly SchuelerRepository _schuelerRepository;
    private readonly FachRepository _fachRepository;
    private readonly LehrerRepository _lehrerRepository;
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly IOptions<NoteConfiguration> _noteConfiguration;

    public ErstelleNoteRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, SchuelerRepository schuelerRepository, FachRepository fachRepository, LehrerRepository lehrerRepository, LehrerAccessor lehrerAccessor, IOptions<NoteConfiguration> noteConfiguration)
    {
        _context = context;
        _schuelerRepository = schuelerRepository;
        _fachRepository = fachRepository;
        _lehrerRepository = lehrerRepository;
        _lehrerAccessor = lehrerAccessor;
        _noteConfiguration = noteConfiguration;
    }

    protected override async Task<ErstelleNoteResponse> HandleAsync(ErstelleNoteRequest request)
    {
        if (request.Wert < _noteConfiguration.Value.MinWert || request.Wert > _noteConfiguration.Value.MaxWert)
        {
            throw new ValidationException($"Der Notenwert muss zwischen {_noteConfiguration.Value.MinWert} und {_noteConfiguration.Value.MaxWert} liegen.");
        }
        
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitKlassenAsync(jwtLehrer.Id);
        var schueler = await _schuelerRepository.LadeSchuelerAsync(request.SchuelerId);
        var fach = await _fachRepository.LadeFachAsync(request.FachId);
        
        lehrer.DarfKlasseVerwalten(schueler.KlasseId);
        lehrer.DarfFachVerwalten(fach.Id);
        
        var note = new Note
        {
            Id = Guid.NewGuid(),
            SchuelerId = schueler.Id,
            FachId = fach.Id,
            Wert = request.Wert,
            Notiz = request.Notiz,
            ErstelltAm = DateTime.UtcNow,
            AngepasstAm = DateTime.UtcNow,
            Schueler = schueler,
            Fach = fach
        };

        var entry = _context.Note.Add(note);
        
        await _context.SaveChangesAsync();

        return new ErstelleNoteResponse
        {
            Note = NoteDto.Convert(entry.Entity) 
        };
    }
}

public class ErstelleNoteRequest : IRequest
{
    public required Guid SchuelerId { get; init; }
    public required Guid FachId { get; init; }
    public required int Wert { get; init; }
    public string? Notiz { get; init; }
}

public class ErstelleNoteResponse : IResponse
{
    public required NoteDto Note { get; init; }
}