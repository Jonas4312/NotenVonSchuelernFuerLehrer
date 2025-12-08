using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

public class SchuelerRepository
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public SchuelerRepository(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Schueler>> LadeAlleSchuelerEinerKlasseAsync(Guid klasseId)
    {
        return await _context.Schueler
            .Where(s => s.KlasseId == klasseId)
            .ToListAsync();
    }
    
    public async Task<Schueler> LadeSchuelerAnIdAsync(Guid schuelerId)
    {
        return await _context.Schueler
            .Include(s => s.Noten)
            .ThenInclude(n => n.Fach)
            .FirstAsync(s => s.Id == schuelerId);
    }
}