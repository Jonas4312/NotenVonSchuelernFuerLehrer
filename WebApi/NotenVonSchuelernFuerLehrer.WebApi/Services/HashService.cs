namespace NotenVonSchuelernFuerLehrer.WebApi.Services;

public class HashService
{
    public string HashPassword(string passwort)
    {
        return BCrypt.Net.BCrypt.HashPassword(passwort);   
    }

    public bool IsValidPassword(string passwort, string passwortHash)
    {
        return BCrypt.Net.BCrypt.Verify(passwort, passwortHash);
    }
}
