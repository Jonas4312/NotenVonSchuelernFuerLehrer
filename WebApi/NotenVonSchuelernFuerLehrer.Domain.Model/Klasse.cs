namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Klasse
{
    public required Guid Id { get; init; }
    public required string Bezeichnung { get; init; }
    public required string Kurzbezeichnung { get; init; }
    
    public required List<Schueler> Schueler { get; init; } = [];
    public required List<Fach> Faecher { get; init; } = [];
}