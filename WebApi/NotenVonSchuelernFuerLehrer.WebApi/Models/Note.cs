namespace NotenVonSchuelernFuerLehrer.WebApi.Models;

public class Note
{
    public int Id { get; set; }
    public int SchuelerId { get; set; }
    public int LehrerId { get; set; }
    public int KlasseId { get; set; }
    public double Wert { get; set; }
    public string? Fach { get; set; }
    public DateTime Datum { get; set; }
}
