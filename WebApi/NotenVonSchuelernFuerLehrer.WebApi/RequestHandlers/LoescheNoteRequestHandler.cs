using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LoescheNoteRequestHandler : BaseRequestHandler<LoescheNoteRequest, LoescheNoteResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly NoteRepository _noteRepository;
    private readonly LehrerRepository _lehrerRepository;
    private readonly LehrerAccessor _lehrerAccessor;

    public LoescheNoteRequestHandler(NotenVonSchuelernFuerLehrerDbContext context, NoteRepository noteRepository, LehrerRepository lehrerRepository, LehrerAccessor lehrerAccessor)
    {
        _context = context;
        _noteRepository = noteRepository;
        _lehrerRepository = lehrerRepository;
        _lehrerAccessor = lehrerAccessor;
    }

    protected override async Task<LoescheNoteResponse> HandleAsync(LoescheNoteRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitFaecherUndKlassenAsync(jwtLehrer.Id);
        var note = await _noteRepository.LadeNoteMitFachUndSchuelerAsync(request.NoteId);
        
        lehrer.DarfFachVerwalten(note.FachId);
        lehrer.DarfKlasseVerwalten(note.Schueler.KlasseId);
        
        _context.Note.Remove(note);
        await _context.SaveChangesAsync();
        
        return new LoescheNoteResponse();
    }
}

public class LoescheNoteRequest : IRequest
{
    public required Guid NoteId { get; init; }
}

public class LoescheNoteResponse : IEmptyResponse;