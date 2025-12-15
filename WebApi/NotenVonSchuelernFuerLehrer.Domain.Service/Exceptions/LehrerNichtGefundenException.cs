namespace NotenVonSchuelernFuerLehrer.Domain.Service.Exceptions;

/// <summary>
/// Exception die geworfen wird, wenn ein Lehrer mit der angegebenen ID nicht gefunden wurde.
/// Dies kann passieren, wenn die Datenbank zurückgesetzt wurde oder der Benutzer gelöscht wurde.
/// </summary>
public class LehrerNichtGefundenException : Exception
{
    public Guid LehrerId { get; }

    public LehrerNichtGefundenException(Guid lehrerId) 
        : base($"Lehrer mit ID {lehrerId} wurde nicht gefunden. Bitte erneut anmelden.")
    {
        LehrerId = lehrerId;
    }
}
