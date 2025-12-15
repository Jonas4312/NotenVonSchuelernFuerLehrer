using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Dtos;

public class SchuelerDto
{
    public required Guid Id { get; init; }
    public required string Vorname { get; init; }
    public required string Nachname { get; init; }
    public required byte[] BildByteArray { get; init; }
    public int AnzahlNoten { get; init; }
    public Guid? KlasseId { get; init; }
    public string? KlasseBezeichnung { get; init; }
    
    public static SchuelerDto Convert(Schueler schueler)
    {
        return new SchuelerDto
        {
            Id = schueler.Id,
            Vorname = schueler.Vorname,
            Nachname = schueler.Nachname,
            BildByteArray = schueler.BildByteArray,
            AnzahlNoten = schueler.Noten?.Count ?? 0,
            KlasseId = schueler.KlasseId,
            KlasseBezeichnung = schueler.Klasse?.Bezeichnung
        };
    }
}