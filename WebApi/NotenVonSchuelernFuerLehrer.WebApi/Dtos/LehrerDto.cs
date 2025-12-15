using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Dtos;

public class LehrerDto
{
    public required Guid Id { get; init; }
    public required string Vorname { get; init; }
    public required string Nachname { get; init; }
    public required string Benutzername { get; init; }
    public required byte[] BildByteArray { get; init; }
    public List<FachDto>? Faecher { get; init; }
    public List<KlasseDto>? Klassen { get; init; }
    
    public static LehrerDto Convert(Lehrer lehrer)
    {
        return new LehrerDto
        {
            Id = lehrer.Id,
            Vorname = lehrer.Vorname,
            Nachname = lehrer.Nachname,
            Benutzername = lehrer.Benutzername,
            BildByteArray = lehrer.BildByteArray,
            Faecher = lehrer.Faecher?.Select(FachDto.Convert).ToList(),
            Klassen = lehrer.Klassen?.Select(KlasseDto.Convert).ToList()
        };
    }
}