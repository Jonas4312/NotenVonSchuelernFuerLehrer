using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Dtos;

public class KlasseDto
{
    public required Guid Id { get; init; }
    public required string Bezeichnung { get; init; }
    public required string Kurzbezeichnung { get; init; }
    public int AnzahlSchueler { get; init; }

    public static KlasseDto Convert(Klasse klasse)
    {
        return new KlasseDto
        {
            Id = klasse.Id,
            Bezeichnung = klasse.Bezeichnung,
            Kurzbezeichnung = klasse.Kurzbezeichnung,
            AnzahlSchueler = klasse.Schueler?.Count ?? 0
        };
    }
}   