using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeNoteEinesSchuelersRequestHandler : BaseRequestHandler<LadeNoteEinesSchuelersRequest, LadeNoteEinesSchuelersResponse>
{
    private readonly NoteRepository _noteRepository;
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly LehrerRepository _lehrerRepository;

    public LadeNoteEinesSchuelersRequestHandler(NoteRepository noteRepository, LehrerAccessor lehrerAccessor, LehrerRepository lehrerRepository)
    {
        _noteRepository = noteRepository;
        _lehrerAccessor = lehrerAccessor;
        _lehrerRepository = lehrerRepository;
    }

    protected override async Task<LadeNoteEinesSchuelersResponse> HandleAsync(LadeNoteEinesSchuelersRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitFaecherUndKlassenAsync(jwtLehrer.Id);
        var note = await _noteRepository.LadeNoteMitFachUndSchuelerAsync(request.NoteId);

        lehrer.DarfKlasseVerwalten(note.Schueler.KlasseId);
        
        return new LadeNoteEinesSchuelersResponse
        {
            Note = NoteDto.Convert(note) 
        };
    }
}

public class LadeNoteEinesSchuelersRequest : IRequest
{
    public required Guid NoteId { get; init; }
}

public class LadeNoteEinesSchuelersResponse : IResponse
{
    public required NoteDto Note { get; init; }
}