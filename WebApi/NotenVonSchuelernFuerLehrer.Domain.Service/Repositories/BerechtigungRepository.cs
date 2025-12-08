using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

public class BerechtigungRepository
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public BerechtigungRepository(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> DarfLehrerKlasseUnterichtenAsync(Guid klasseId, Guid lehrerId)
    {
        var lehrer = await _context.Lehrer
            .Include(l => l.Faecher)
            .ThenInclude(f => f.Klassen)
            .FirstAsync(l => l.Id == lehrerId);

        return lehrer.Faecher.Any(fach => fach.Klassen.Any(klasse => klasse.Id == klasseId));
    }
    
    public async Task<bool> DarfLehrerFachUnterichtenAsync(Guid fachId, Guid lehrerId)
    {
        var lehrer = await _context.Lehrer
            .Include(l => l.Faecher)
            .FirstAsync(l => l.Id == lehrerId);

        return lehrer.Faecher.Any(fach => fach.Id == fachId);
    }
}