using Microsoft.EntityFrameworkCore;
using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Services;

public class SeedDataService
{
    private readonly NotenVonSchuelernFuerLehrerDbContext _context;
    private readonly HashService _hashService;
    private readonly HttpClient _httpClient;
    
    public SeedDataService(NotenVonSchuelernFuerLehrerDbContext context, HashService hashService, HttpClient httpClient)
    {
        _context = context;
        _hashService = hashService;
        _httpClient = httpClient;
    }

    public async Task SeedAsync()
    {
        var matheFach = AddOrCreateFach("Mathematik", "Mathe");
        var deutschFach = AddOrCreateFach("Deutsch", "Deutsch");
        var englischFach = AddOrCreateFach("Englisch", "Englisch");
        
        var klasse10A = AddOrCreateKlasse("10A", "10A");

        AddOrCreateFachZuKlasse(matheFach, klasse10A);
        AddOrCreateFachZuKlasse(deutschFach, klasse10A);
        AddOrCreateFachZuKlasse(englischFach, klasse10A);
        
        var maxMustermannSchueler = AddOrCreateSchueler(klasse10A, "Max", "Mustermann", await DownloadImageAsByteArrayAsync());
        
        var mariaSchmidtLehrer = AddOrCreateLehrer("Maria", "Schmidt", "mschmidt", "Passwort123", await DownloadImageAsByteArrayAsync());
        
        AddOrCreateFachZuLehrer(matheFach, mariaSchmidtLehrer);
        AddOrCreateFachZuLehrer(deutschFach, mariaSchmidtLehrer);
        AddOrCreateFachZuLehrer(englischFach, mariaSchmidtLehrer);

        AddOrCreateNote(maxMustermannSchueler, matheFach, 4, DateTime.Parse("2023-09-01"), DateTime.Parse("2023-09-01"));
        AddOrCreateNote(maxMustermannSchueler, matheFach, 4, DateTime.Parse("2023-09-02"), DateTime.Parse("2023-09-02"));
        AddOrCreateNote(maxMustermannSchueler, deutschFach, 3, DateTime.Parse("2023-09-02"), DateTime.Parse("2023-09-02"));
        AddOrCreateNote(maxMustermannSchueler, englischFach, 2, DateTime.Parse("2023-09-03"), DateTime.Parse("2023-09-03"));

        await _context.SaveChangesAsync();
    }

    private Fach AddOrCreateFach(string bezeichnung, string kurzbezeichnung)
    {
        var fach = _context.Fach
            .Include(f => f.Klassen)
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
            Klassen = [],
            Lehrer = []
        };
        
        var entry = _context.Fach.Add(newFach);
        return entry.Entity;
    }
    
    private Klasse AddOrCreateKlasse(string bezeichnung, string kurzbezeichnung)
    {
        var klasse = _context.Klasse
            .Include(k => k.Schueler)
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
            Faecher = []
        };
        
        var entry = _context.Klasse.Add(newKlasse);
        return entry.Entity;
    }
    
    private void AddOrCreateFachZuKlasse(Fach fach, Klasse klasse)
    {
        if(!fach.Klassen.Contains(klasse))
        {
            fach.Klassen.Add(klasse);
        }
        
        if(!klasse.Faecher.Contains(fach))
        {
            klasse.Faecher.Add(fach);
        }
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
            Faecher = []
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
        var response = await _httpClient.GetAsync("/50/50");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }
}
