using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Services;

public class SeedDataService
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly HashService _hashService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    public SeedDataService(NotenVonSchuelernFuerLehrerDbContext context, HashService hashService, HttpClient httpClient, IConfiguration configuration)
    {
        _context = context;
        _hashService = hashService;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        var seedMode = _configuration["SeedMode"]?.ToLowerInvariant() ?? "full";
        
        if (seedMode == "minimal")
        {
            await SeedMinimalAsync();
        }
        else
        {
            await SeedFullAsync();
        }
    }

    /// <summary>
    /// Minimal-Seeding: Nur ein Admin-Lehrer, keine Klassen, Fächer oder Schüler
    /// </summary>
    private async Task SeedMinimalAsync()
    {
        // Nur einen Admin-Lehrer erstellen
        AddOrCreateLehrer("Admin", "Lehrer", "admin", "Admin123!", await DownloadImageAsByteArrayAsync());
        
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Full-Seeding: Alle Demo-Daten (5 Lehrer, 3 Klassen, 89 Schüler, Noten)
    /// </summary>
    private async Task SeedFullAsync()
    {
        var matheFach = AddOrCreateFach("Mathematik", "MA");
        var deutschFach = AddOrCreateFach("Deutsch", "DE");
        var englischFach = AddOrCreateFach("Englisch", "EN");
        
        var klasse10A = AddOrCreateKlasse("10A", "10A");
        var klasse10B = AddOrCreateKlasse("10B", "10B");
        var klasse9A = AddOrCreateKlasse("9A", "9A");
        
        // Lehrer 1: Maria Schmidt - unterrichtet alle Fächer in allen Klassen
        var mariaSchmidtLehrer = AddOrCreateLehrer("Maria", "Schmidt", "mschmidt", "Passwort123", await DownloadImageAsByteArrayAsync());
        AddOrCreateFachZuLehrer(matheFach, mariaSchmidtLehrer);
        AddOrCreateFachZuLehrer(deutschFach, mariaSchmidtLehrer);
        AddOrCreateFachZuLehrer(englischFach, mariaSchmidtLehrer);
        AddOrCreateKlasseZuLehrer(klasse10A, mariaSchmidtLehrer);
        AddOrCreateKlasseZuLehrer(klasse10B, mariaSchmidtLehrer);
        AddOrCreateKlasseZuLehrer(klasse9A, mariaSchmidtLehrer);
        
        // Lehrer 2: Thomas Müller - unterrichtet nur Mathe in 10A und 10B
        var thomasMuellerLehrer = AddOrCreateLehrer("Thomas", "Müller", "tmueller", "Passwort123", await DownloadImageAsByteArrayAsync());
        AddOrCreateFachZuLehrer(matheFach, thomasMuellerLehrer);
        AddOrCreateKlasseZuLehrer(klasse10A, thomasMuellerLehrer);
        AddOrCreateKlasseZuLehrer(klasse10B, thomasMuellerLehrer);
        
        // Lehrer 3: Sabine Weber - unterrichtet Deutsch und Englisch in 9A
        var sabineWeberLehrer = AddOrCreateLehrer("Sabine", "Weber", "sweber", "Passwort123", await DownloadImageAsByteArrayAsync());
        AddOrCreateFachZuLehrer(deutschFach, sabineWeberLehrer);
        AddOrCreateFachZuLehrer(englischFach, sabineWeberLehrer);
        AddOrCreateKlasseZuLehrer(klasse9A, sabineWeberLehrer);
        
        // Lehrer 4: Klaus Fischer - neu eingestellt, hat noch keine Klassen zugeordnet
        var klausFischerLehrer = AddOrCreateLehrer("Klaus", "Fischer", "kfischer", "Passwort123", await DownloadImageAsByteArrayAsync());
        // Keine Klassen zugeordnet - sieht keine Schüler
        
        // Lehrer 5: Petra Hofmann - unterrichtet nur Englisch in 10A
        var petraHofmannLehrer = AddOrCreateLehrer("Petra", "Hofmann", "phofmann", "Passwort123", await DownloadImageAsByteArrayAsync());
        AddOrCreateFachZuLehrer(englischFach, petraHofmannLehrer);
        AddOrCreateKlasseZuLehrer(klasse10A, petraHofmannLehrer);

        // 35 Schüler für Klasse 10A
        var schuelerNamen10A = new[]
        {
            ("Max", "Mustermann"), ("Anna", "Müller"), ("Leon", "Schmidt"), ("Emma", "Fischer"),
            ("Luca", "Weber"), ("Mia", "Meyer"), ("Noah", "Wagner"), ("Sophie", "Becker"),
            ("Elias", "Hoffmann"), ("Hannah", "Schäfer"), ("Ben", "Koch"), ("Lena", "Bauer"),
            ("Paul", "Richter"), ("Laura", "Klein"), ("Finn", "Wolf"), ("Marie", "Schröder"),
            ("Jonas", "Neumann"), ("Lea", "Schwarz"), ("Luis", "Zimmermann"), ("Emilia", "Braun"),
            ("Felix", "Krüger"), ("Johanna", "Hofmann"), ("Tim", "Hartmann"), ("Clara", "Lange"),
            ("Nico", "Schmitt"), ("Amelie", "Werner"), ("David", "Schmitz"), ("Sarah", "Krause"),
            ("Moritz", "Meier"), ("Lara", "Lehmann"), ("Jan", "Schulz"), ("Luisa", "Maier"),
            ("Tom", "Köhler"), ("Nele", "Herrmann"), ("Erik", "König")
        };
        
        // 29 Schüler für Klasse 10B
        var schuelerNamen10B = new[]
        {
            ("Alexander", "Berger"), ("Julia", "Scholz"), ("Maximilian", "Huber"), ("Katharina", "Fuchs"),
            ("Sebastian", "Vogel"), ("Christina", "Roth"), ("Florian", "Beck"), ("Vanessa", "Lorenz"),
            ("Philipp", "Frank"), ("Sabrina", "Albrecht"), ("Michael", "Simon"), ("Jennifer", "Ludwig"),
            ("Patrick", "Böhm"), ("Melanie", "Winter"), ("Daniel", "Kraus"), ("Nicole", "Schuster"),
            ("Tobias", "Jäger"), ("Stephanie", "Peters"), ("Christian", "Sommer"), ("Sandra", "Stein"),
            ("Matthias", "Haas"), ("Nadine", "Graf"), ("Stefan", "Heinrich"), ("Jasmin", "Brandt"),
            ("Andreas", "Schreiber"), ("Bianca", "Dietrich"), ("Markus", "Kuhn"), ("Tanja", "Engel"),
            ("Kevin", "Pohl")
        };
        
        // 25 Schüler für Klasse 9A
        var schuelerNamen9A = new[]
        {
            ("Oliver", "Sauer"), ("Lisa", "Arnold"), ("Dennis", "Wolff"), ("Marina", "Pfeiffer"),
            ("Marcel", "Weiß"), ("Kerstin", "Günther"), ("Dominik", "Baumann"), ("Petra", "Keller"),
            ("Simon", "Möller"), ("Claudia", "Schmid"), ("Benjamin", "Schäfer"), ("Martina", "Hahn"),
            ("Fabian", "Schubert"), ("Monika", "Vogt"), ("Julian", "Friedrich"), ("Simone", "Kraft"),
            ("Robin", "Lindner"), ("Diana", "Böttcher"), ("Alexander", "Krämer"), ("Manuela", "Busch"),
            ("Marco", "Ritter"), ("Daniela", "Bergmann"), ("Sven", "Bauer"), ("Franziska", "Wendt"),
            ("Christoph", "Otto")
        };
        
        var random = new Random(42); // Fixed seed für konsistente Daten
        var faecher = new[] { matheFach, deutschFach, englischFach };
        
        // Schüler für Klasse 10A erstellen
        foreach (var (vorname, nachname) in schuelerNamen10A)
        {
            var schueler = AddOrCreateSchueler(klasse10A, vorname, nachname, await DownloadImageAsByteArrayAsync());
            await AddRandomNotenAsync(schueler, faecher, random);
        }
        
        // Schüler für Klasse 10B erstellen
        foreach (var (vorname, nachname) in schuelerNamen10B)
        {
            var schueler = AddOrCreateSchueler(klasse10B, vorname, nachname, await DownloadImageAsByteArrayAsync());
            await AddRandomNotenAsync(schueler, faecher, random);
        }
        
        // Schüler für Klasse 9A erstellen
        foreach (var (vorname, nachname) in schuelerNamen9A)
        {
            var schueler = AddOrCreateSchueler(klasse9A, vorname, nachname, await DownloadImageAsByteArrayAsync());
            await AddRandomNotenAsync(schueler, faecher, random);
        }

        await _context.SaveChangesAsync();
    }
    
    private Task AddRandomNotenAsync(Schueler schueler, Fach[] faecher, Random random)
    {
        foreach (var fach in faecher)
        {
            var anzahlNoten = random.Next(1, 4); // 1-3 Noten pro Fach
            for (int i = 0; i < anzahlNoten; i++)
            {
                var note = random.Next(1, 7); // Note 1-6
                var datum = DateTime.Parse("2023-09-01").AddDays(random.Next(0, 90));
                AddOrCreateNote(schueler, fach, note, datum, datum);
            }
        }
        return Task.CompletedTask;
    }

    private Fach AddOrCreateFach(string bezeichnung, string kurzbezeichnung)
    {
        var fach = _context.Fach
            .Include(l => l.Lehrer)
            .FirstOrDefault(f => f.Bezeichnung == bezeichnung);
        
        if(fach != null)
        {
            return fach;
        }
        
        var newFach = new Fach
        {
            Id = Guid.NewGuid(),
            Bezeichnung = bezeichnung,
            Kurzbezeichnung = kurzbezeichnung,
            Noten = [],
            Lehrer = []
        };
        
        var entry = _context.Fach.Add(newFach);
        return entry.Entity;
    }
    
    private Klasse AddOrCreateKlasse(string bezeichnung, string kurzbezeichnung)
    {
        var klasse = _context.Klasse
            .Include(k => k.Schueler)
            .Include(k => k.Lehrer)
            .FirstOrDefault(k => k.Bezeichnung == bezeichnung);
        
        if(klasse != null)
        {
            return klasse;
        }
        
        var newKlasse = new Klasse
        {
            Id = Guid.NewGuid(),
            Bezeichnung = bezeichnung,
            Kurzbezeichnung = kurzbezeichnung,
            Schueler = [],
            Lehrer = []
        };
        
        var entry = _context.Klasse.Add(newKlasse);
        return entry.Entity;
    }
    
    private Schueler AddOrCreateSchueler(Klasse klasse, string vorname, string nachname, byte[] bildByteArray)
    {
        var schueler = _context.Schueler.FirstOrDefault(s => s.Vorname == vorname && s.Nachname == nachname && s.KlasseId == klasse.Id);
        
        if(schueler != null)
        {
            return schueler;
        }
        
        var newSchueler = new Schueler
        {
            Id = Guid.NewGuid(),
            KlasseId = klasse.Id,
            Vorname = vorname,
            Nachname = nachname,
            BildByteArray = bildByteArray,
            Noten = [],
            Klasse = klasse
        };
        
        var entry = _context.Schueler.Add(newSchueler);
        klasse.Schueler.Add(entry.Entity);
        return entry.Entity;
    }
    
    private Lehrer AddOrCreateLehrer(string vorname, string nachname, string benutzername, string passwort, byte[] bildByteArray)
    {
        var lehrer = _context.Lehrer
            .Include(l => l.Faecher)
            .Include(l => l.Klassen)
            .FirstOrDefault(l => l.Benutzername == benutzername);
        
        if(lehrer != null)
        {
            return lehrer;
        }
        
        var newLehrer = new Lehrer
        {
            Id = Guid.NewGuid(),
            Vorname = vorname,
            Nachname = nachname,
            Benutzername = benutzername,
            PasswortHash = _hashService.HashPassword(passwort),
            BildByteArray = bildByteArray,
            Faecher = [],
            Klassen = []
        };
        
        var entry = _context.Lehrer.Add(newLehrer);
        return entry.Entity;
    }
    
    private void AddOrCreateFachZuLehrer(Fach fach, Lehrer lehrer)
    {
        if(!fach.Lehrer.Contains(lehrer))
        {
            fach.Lehrer.Add(lehrer);
        }
        
        if(!lehrer.Faecher.Contains(fach))
        {
            lehrer.Faecher.Add(fach);
        }
    }
    
    private void AddOrCreateKlasseZuLehrer(Klasse klasse, Lehrer lehrer)
    {
        if(!klasse.Lehrer.Contains(lehrer))
        {
            klasse.Lehrer.Add(lehrer);
        }
        
        if(!lehrer.Klassen.Contains(klasse))
        {
            lehrer.Klassen.Add(klasse);
        }
    }
    
    private void AddOrCreateNote(Schueler schueler, Fach fach, int wert, DateTime erstelltAm, DateTime angepasstAm)
    {
        var note = _context.Note.FirstOrDefault(n => n.SchuelerId == schueler.Id && n.FachId == fach.Id && n.Wert == wert && n.ErstelltAm == erstelltAm && n.AngepasstAm == angepasstAm);
        
        if(note != null)
        {
            return;
        }
        
        var newNote = new Note
        {
            Id = Guid.NewGuid(),
            SchuelerId = schueler.Id,
            FachId = fach.Id,
            Wert = wert,
            ErstelltAm = erstelltAm,
            AngepasstAm = angepasstAm,
            Schueler = schueler,
            Fach = fach
        };
        
        var entry = _context.Note.Add(newNote);
        schueler.Noten.Add(entry.Entity);
        fach.Noten.Add(entry.Entity);
    }
    
    private async Task<byte[]> DownloadImageAsByteArrayAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/50/50");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch
        {
            // Falls kein Internetzugang, erzeuge ein einfaches Placeholder-Bild (1x1 PNG)
            return Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");
        }
    }
}
