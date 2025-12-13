using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Dtos;

public class FachDto
{
    public required Guid Id { get; init; }
    public required string Bezeichnung { get; init; }
    public required string Kurzbezeichnung { get; init; }
    
    public static FachDto Convert(Fach fach)
    {
        return new FachDto
        {
            Id = fach.Id,
            Bezeichnung = fach.Bezeichnung,
            Kurzbezeichnung = fach.Kurzbezeichnung
        };
    }
}