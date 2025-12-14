using Microsoft.Extensions.Options;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Configuration;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Exceptions;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class AenderNoteRequestHandler : BaseRequestHandler<AenderNoteRequest, AenderNoteResponse>
{
    private readonly NoteRepository _noteRepository;
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly LehrerRepository _lehrerRepository;
    private readonly IOptions<NoteConfiguration> _noteConfiguration;

    public AenderNoteRequestHandler(NoteRepository noteRepository, NotenVonSchuelernFuerLehrerDbContext context, LehrerAccessor lehrerAccessor, LehrerRepository lehrerRepository, IOptions<NoteConfiguration> noteConfiguration)
    {
        _noteRepository = noteRepository;
        _context = context;
        _lehrerAccessor = lehrerAccessor;
        _lehrerRepository = lehrerRepository;
        _noteConfiguration = noteConfiguration;
    }

    protected override async Task<AenderNoteResponse> HandleAsync(AenderNoteRequest request)
    {
        if (request.NeueNote < _noteConfiguration.Value.MinWert || request.NeueNote > _noteConfiguration.Value.MaxWert)
        {
            throw new ValidationException($"Der Notenwert muss zwischen {_noteConfiguration.Value.MinWert} und {_noteConfiguration.Value.MaxWert} liegen.");
        }
        
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitFaecherUndKlassenAsync(jwtLehrer.Id);
        var note = await _noteRepository.LadeNoteMitFachUndSchuelerAsync(request.NoteId);
        
        lehrer.DarfFachVerwalten(note.FachId);
        lehrer.DarfKlasseVerwalten(note.Schueler.KlasseId);
        
        note.Wert = request.NeueNote;
        note.AngepasstAm = DateTime.UtcNow;
        note.Notiz = request.Notiz;
        
        await _context.SaveChangesAsync();
        
        return new AenderNoteResponse
        {
            Note = NoteDto.Convert(note)
        };
    }
}

public class AenderNoteRequest : IRequest
{
    public required Guid NoteId { get; init; }
    public required int NeueNote { get; init; }
    public string? Notiz { get; init; }
}

public class AenderNoteResponse : IResponse
{
    public required NoteDto Note { get; init; }
}