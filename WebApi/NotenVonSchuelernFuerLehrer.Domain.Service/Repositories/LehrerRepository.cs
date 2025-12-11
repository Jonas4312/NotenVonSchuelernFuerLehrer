using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

public class LehrerRepository
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public LehrerRepository(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }

    public async Task<Lehrer> LadeLehrerAnIdAsync(Guid lehrerId)
    {
        return await _context.Lehrer
            .Include(l => l.Faecher)
            .ThenInclude(f => f.Klassen)
            .FirstAsync(l => l.Id == lehrerId);
    }
    
    public async Task<Lehrer?> LadeLehrerAnBenutzernameAsync(string benutzername)
    {
        return await _context.Lehrer.FirstOrDefaultAsync(l => l.Benutzername == benutzername);
    }
}