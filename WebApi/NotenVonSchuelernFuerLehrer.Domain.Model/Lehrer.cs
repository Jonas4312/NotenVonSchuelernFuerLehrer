namespace NotenVonSchuelernFuerLehrer.Domain.Model;

public class Lehrer
{
    public required Guid Id { get; init; }
    public required string Vorname { get; init; }
    public required string Nachname { get; init; }
    public required string Benutzername { get; init; }
    public required string PasswortHash { get; init; }
    public required byte[] BildByteArray { get; init; }
    
    public required List<Fach> Faecher { get; init; }
}