using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

public class KlasseRepository
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public KlasseRepository(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }
    
    public async Task<Klasse> LadeKlasseAsync(Guid klasseId)
    {
        return await _context.Klasse.FirstAsync(k => k.Id == klasseId);
    }
}