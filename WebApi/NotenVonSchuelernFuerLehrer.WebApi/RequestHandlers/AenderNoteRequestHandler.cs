using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class AenderNoteRequestHandler : BaseRequestHandler<AenderNoteRequest, AenderNoteResponse>
{
    private readonly NoteRepository _noteRepository;
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly LehrerRepository _lehrerRepository;

    public AenderNoteRequestHandler(NoteRepository noteRepository, NotenVonSchuelernFuerLehrerDbContext context, LehrerAccessor lehrerAccessor, LehrerRepository lehrerRepository)
    {
        _noteRepository = noteRepository;
        _context = context;
        _lehrerAccessor = lehrerAccessor;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<AenderNoteResponse> HandleAsync(AenderNoteRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitFaecherUndKlassenAsync(jwtLehrer.Id);
        var note = await _noteRepository.LadeNoteMitFachUndSchuelerAsync(request.NoteId);
        
        lehrer.DarfFachVerwalten(note.FachId);
        lehrer.DarfKlasseVerwalten(note.Schueler.KlasseId);
        
        note.Wert = request.NeueNote;
        note.AngepasstAm = DateTime.UtcNow;
        
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
}

public class AenderNoteResponse : IResponse
{
    public required NoteDto Note { get; init; }
}