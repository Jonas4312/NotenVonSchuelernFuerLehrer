using NotenVonSchuelernFuerLehrer.Domain.Model;

namespace NotenVonSchuelernFuerLehrer.WebApi.Dtos;

public class NoteDto
{
    public required Guid Id { get; init; }
    public required int Wert { get; init; }
    public string? Notiz { get; init; }
    public required DateTime ErstelltAm { get; init; }
    public required DateTime AngepasstAm { get; init; }
    public required FachDto Fach { get; init; }

    public static NoteDto Convert(Note note)
    {
        return new NoteDto
        {
            Id = note.Id,
            Wert = note.Wert,
            Notiz = note.Notiz,
            ErstelltAm = note.ErstelltAm,
            AngepasstAm = note.AngepasstAm,
            Fach = FachDto.Convert(note.Fach)
        };
    }
}