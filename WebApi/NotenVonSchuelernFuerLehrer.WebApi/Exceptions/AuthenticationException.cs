namespace NotenVonSchuelernFuerLehrer.WebApi.Exceptions;

/// <summary>
/// Exception die geworfen wird, wenn der authentifizierte Benutzer nicht mehr gültig ist
/// (z.B. wenn die Datenbank zurückgesetzt wurde und der Benutzer nicht mehr existiert).
/// </summary>
public class AuthenticationException : Exception
{
    public AuthenticationException(string message = "Der Benutzer ist nicht mehr gültig. Bitte erneut anmelden.") 
        : base(message)
    {
    }
}
