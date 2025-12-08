namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Fach
{
    public required Guid Id { get; init; }
    public required string Bezeichnung { get; init; }
    public required string Kurzbezeichnung { get; init; }
    
    public required List<Lehrer> Lehrer { get; init; }
    public required List<Klasse> Klassen { get; init; }
    public required List<Note> Noten { get; init; }
}