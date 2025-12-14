using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Dtos;

namespace NotenVonSchuelernFuerLehrer.WebApi.RequestHandlers;

public class LadeFaecherRequestHandler : BaseRequestHandler<LadeFaecherRequest, LadeFaecherResponse>
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public LadeFaecherRequestHandler(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    protected override async Task<LadeFaecherResponse> HandleAsync(LadeFaecherRequest request)
    {
        var faecher = await _context.Fach.ToListAsync();
        
        return new LadeFaecherResponse
        {
            Faecher = faecher.Select(FachDto.Convert).ToList()
        };
    }
}

public class LadeFaecherRequest : IRequest;

public class LadeFaecherResponse : IResponse
{
    public required List<FachDto> Faecher { get; init; }
}
