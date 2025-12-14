namespace NotenVonSchuelernFuerLehrer.WebApi.Configuration;

public class JwtConfiguration
{
    public required string Key { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
}