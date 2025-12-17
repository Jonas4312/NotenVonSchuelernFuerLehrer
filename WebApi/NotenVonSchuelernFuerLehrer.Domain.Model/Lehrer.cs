namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Lehrer
{
    public required Guid Id { get; init; }
    public required string Vorname { get; set; }
    public required string Nachname { get; set; }
    public required string Benutzername { get; set; }
    public required string PasswortHash { get; set; }
    public required byte[] BildByteArray { get; set; }
    public bool IsDeleted { get; set; }
    
    public required List<Fach> Faecher { get; init; } = [];
    public required List<Klasse> Klassen { get; init; } = [];
    
    public void DarfFachVerwalten(Guid fachId)
    {
        if (Faecher.All(f => f.Id != fachId))
        {
            throw new UnauthorizedAccessException("Lehrer darf dieses Fach nicht verwalten.");
        }
    }
    
    public void DarfKlasseVerwalten(Guid klasseId)
    {
        if (Klassen.All(k => k.Id != klasseId))
        {
            throw new UnauthorizedAccessException("Lehrer darf diese Klasse nicht verwalten.");
        }
    }
}