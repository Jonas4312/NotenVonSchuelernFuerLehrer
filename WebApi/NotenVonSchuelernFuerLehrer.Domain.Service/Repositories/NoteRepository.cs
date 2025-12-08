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
    
    public async Task<Note> LadeNoteAnIdAsync(Guid noteId)
    {
        return await _context.Note.FirstAsync(n => n.Id == noteId);
    }
}