using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.Domain.Service.Exceptions;

namespace NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

public class LehrerRepository
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public LehrerRepository(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    public async Task<Lehrer> LadeLehrerAsync(Guid lehrerId)
    {
        var lehrer = await _context.Lehrer.FirstOrDefaultAsync(l => l.Id == lehrerId);
        if (lehrer is null)
        {
            throw new LehrerNichtGefundenException(lehrerId);
        }
        return lehrer;
    }

    public async Task<Lehrer> LadeLehrerMitKlassenAsync(Guid lehrerId)
    {
        var lehrer = await _context.Lehrer
            .Include(l => l.Klassen)
            .ThenInclude(k => k.Schueler)
            .FirstOrDefaultAsync(l => l.Id == lehrerId);
        
        if (lehrer is null)
        {
            throw new LehrerNichtGefundenException(lehrerId);
        }
        return lehrer;
    }
    
    public async Task<Lehrer?> LadeLehrerMitKlassenAsync(string benutzername)
    {
        return await _context.Lehrer
            .Include(l => l.Klassen)
            .ThenInclude(k => k.Schueler)
            .FirstOrDefaultAsync(l => l.Benutzername == benutzername);
    }
}