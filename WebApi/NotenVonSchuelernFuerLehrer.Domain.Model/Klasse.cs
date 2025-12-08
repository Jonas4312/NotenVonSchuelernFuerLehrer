namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Klasse
{
    public required Guid Id { get; init; }
    public required string Bezeichnung { get; init; }
    public required string Kurzbezeichnung { get; init; }
    
    public required List<Schueler> Schueler { get; init; }
    public required List<Fach> Faecher { get; init; }
    
    public async Task<Schueler> CreateSchuelerAsync(NotenVonSchuelernFuerLehrerDbContext context, string vorname, string nachname, byte[] bildByteArray)
    {
        var schueler = new Schueler
        {
            Id = Guid.NewGuid(),
            KlasseId = Id,
            Vorname = vorname,
            Nachname = nachname,
            BildByteArray = bildByteArray,
            Noten = [],
            Klasse = this
        };
        Schueler.Add(schueler);
        var entry = await context.Schueler.AddAsync(schueler);
        return entry.Entity;
    }
}