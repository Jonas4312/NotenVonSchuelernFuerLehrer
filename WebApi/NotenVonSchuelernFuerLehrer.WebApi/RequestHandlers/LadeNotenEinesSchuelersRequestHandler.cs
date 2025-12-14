using NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;
using NotenVonSchuelernFuerLehrer.WebApi.Services;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeNotenEinesSchuelersRequestHandler : BaseRequestHandler<LadeNotenEinesSchuelersRequest, LadeNotenEinesSchuelersResponse>
{
    private readonly LehrerAccessor _lehrerAccessor;
    private readonly NoteRepository _noteRepository;
    private readonly LehrerRepository _lehrerRepository;
    private readonly SchuelerRepository _schuelerRepository;

    public LadeNotenEinesSchuelersRequestHandler(LehrerAccessor lehrerAccessor, NoteRepository noteRepository, LehrerRepository lehrerRepository, SchuelerRepository schuelerRepository)
    {
        _lehrerAccessor = lehrerAccessor;
        _noteRepository = noteRepository;
        _lehrerRepository = lehrerRepository;
        _schuelerRepository = schuelerRepository;
    }

    protected override async Task<LadeNotenEinesSchuelersResponse> HandleAsync(LadeNotenEinesSchuelersRequest request)
    {
        var jwtLehrer = _lehrerAccessor.ErmittleLehrerJwt();
        var lehrer = await _lehrerRepository.LadeLehrerMitFaecherUndKlassenAsync(jwtLehrer.Id);
        var noten = await _noteRepository.LadeNotenEinesSchuelersMitFachUndSchuelerAsync(request.SchuelerId);
        var schueler = await _schuelerRepository.LadeSchuelerAsync(request.SchuelerId);
        
        lehrer.DarfKlasseVerwalten(schueler.KlasseId);
        
        return new LadeNotenEinesSchuelersResponse
        {
            Noten = noten
                .Select(NoteDto.Convert)
                .ToList()
        };
    }
}

public class LadeNotenEinesSchuelersRequest : IRequest
{
    public required Guid SchuelerId { get; init; }
}

public class LadeNotenEinesSchuelersResponse : IResponse
{ 
    public required List<NoteDto> Noten { get; init; }
}