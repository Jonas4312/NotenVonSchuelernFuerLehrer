namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Klasse
{
    public required Guid Id { get; init; }
    public required string Bezeichnung { get; set; }
    public required string Kurzbezeichnung { get; set; }
    
    public required List<Schueler> Schueler { get; init; } = [];
    public required List<Lehrer> Lehrer { get; init; } = [];
}