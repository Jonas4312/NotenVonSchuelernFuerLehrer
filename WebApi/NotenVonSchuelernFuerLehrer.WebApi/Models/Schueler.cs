namespace NotenVonSchuelernFuerLehrer.WebApi.Models;

public class Schueler
{
    public int Id { get; set; }
    public string? Vorname { get; set; }
    public string? Nachname { get; set; }
    public int KlasseId { get; set; }
}
