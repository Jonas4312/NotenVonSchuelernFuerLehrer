using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

public class FachRepository
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public FachRepository(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }
    
    public async Task<Fach> LadeFachAsync(Guid fachId)
    {
        return await _context.Fach.FirstAsync(f => f.Id == fachId);
    }
}