using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Services;

public class TestdatenAnlegenService
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly HashService _hashService;

    public TestdatenAnlegenService(NotenVonSchuelernFuerLehrerDbContext context, HashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }

    public async Task SeedAsync()
    {
        if (_context.Lehrer.Any())
        {
            return;
        }

        var fachEntry = _context.Fach.Add(new Fach
        {
            Id = Guid.NewGuid(),
            Bezeichnung = "Mathematik",
            Kurzbezeichnung = "Mathe",
            Noten = [],
            Klassen = [],
            Lehrer = []
        });

        var klasseEntry = _context.Klasse.Add(new Klasse
        {
            Id = Guid.NewGuid(),
            Bezeichnung = "10A",
            Kurzbezeichnung = "10A",
            Schueler = [],
            Faecher = []
        });

        klasseEntry.Entity.Faecher.Add(fachEntry.Entity);
        fachEntry.Entity.Klassen.Add(klasseEntry.Entity);

        var schuelerEntry = _context.Schueler.Add(new Schueler
        {
            Id = Guid.NewGuid(),
            KlasseId = klasseEntry.Entity.Id,
            Vorname = "Max",
            Nachname = "Mustermann",
            BildByteArray = [],
            Klasse = klasseEntry.Entity,
            Noten = [],
        });

        klasseEntry.Entity.Schueler.Add(schuelerEntry.Entity);

        var lehrerEntry = _context.Lehrer.Add(new Lehrer
        {
            Id = Guid.NewGuid(),
            Vorname = "Maria",
            Nachname = "Schmidt",
            Benutzername = "mschmidt",
            PasswortHash = _hashService.HashPassword("Passwort123Passwort123"),
            BildByteArray = [],
            Faecher = []
        });

        lehrerEntry.Entity.Faecher.Add(fachEntry.Entity);
        fachEntry.Entity.Lehrer.Add(lehrerEntry.Entity);

        await _context.SaveChangesAsync();
    }
}
