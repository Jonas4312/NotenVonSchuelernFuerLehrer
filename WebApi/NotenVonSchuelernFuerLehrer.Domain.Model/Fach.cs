namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Fach
{
    public required Guid Id { get; init; }
    public required string Bezeichnung { get; set; }
    public required string Kurzbezeichnung { get; set; }

    public required List<Lehrer> Lehrer { get; init; } = [];
    public required List<Note> Noten { get; init; } = [];
}