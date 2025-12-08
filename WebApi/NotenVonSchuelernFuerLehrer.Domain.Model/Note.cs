namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Note
{
    public required Guid Id { get; init; }
    public required Guid SchuelerId { get; init; }
    public required Guid FachId { get; init; }
    public required int Wert { get; init; }
    public required DateTime ErstelltAm { get; init; }
    public required DateTime AngepasstAm { get; init; }
    
    public required Schueler Schueler { get; init; }
    public required Fach Fach { get; init; }
}