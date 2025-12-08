namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Schueler
{
    public required Guid Id { get; init; }
    public required Guid KlasseId { get; init; }
    public required string Vorname { get; init; }
    public required string Nachname { get; init; }
    public required byte[] BildByteArray { get; init; }
    
    public required Klasse Klasse { get; init; }
    public required List<Note> Noten { get; init; }
    
    public async Task<Note> CreateNoteAsync(NotenVonSchuelernFuerLehrerDbContext context, Fach fach, int wert)
    {
        var note = new Note
        {
            Id = Guid.NewGuid(),
            SchuelerId = Id,
            FachId = fach.Id,
            Wert = wert,
            ErstelltAm = DateTime.UtcNow,
            AngepasstAm = DateTime.UtcNow,
            Schueler = this,
            Fach = fach
        };
        Noten.Add(note);
        fach.Noten.Add(note);
        var entry = await context.Note.AddAsync(note);
        return entry.Entity;
    }
}