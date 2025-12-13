using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.Domain.Service.Repositories;

public class NoteRepository
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;

    public NoteRepository(NotenVonSchuelernFuerLehrerDbContext context)
    {
        _context = context;
    }
    
    public async Task<Note> LadeNoteMitFachUndSchuelerAsync(Guid noteId)
    {
        return await _context.Note
            .Include(n => n.Fach)
            .Include(n => n.Schueler)
            .FirstAsync(n => n.Id == noteId);
    }
    
    public async Task<List<Note>> LadeNotenEinesSchuelersMitFachUndSchuelerAsync(Guid schuelerId)
    {
        return await _context.Note
            .Include(n => n.Fach)
            .Include(n => n.Schueler)
            .Where(n => n.SchuelerId == schuelerId)
            .ToListAsync();
    }
}