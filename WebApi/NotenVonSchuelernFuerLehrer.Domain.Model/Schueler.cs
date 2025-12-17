namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Schueler
{
    public required Guid Id { get; init; }
    public required Guid KlasseId { get; init; }
    public required string Vorname { get; set; }
    public required string Nachname { get; set; }
    public required byte[] BildByteArray { get; set; }
    public bool IsDeleted { get; set; }
    
    public required Klasse Klasse { get; init; }
    public required List<Note> Noten { get; init; } = [];
}