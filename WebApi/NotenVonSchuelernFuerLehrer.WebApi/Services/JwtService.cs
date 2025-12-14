using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotenVonSchuelernFuerLehrer.WebApi.Configuration;

namespace NotenVonSchuelernFuerLehrer.WebApi.Services;

public class JwtService
{
    private readonly IOptions<JwtConfiguration> _jwtConfiguration;

    public JwtService(IOptions<JwtConfiguration> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration;
    }

    public string GenerateToken(JwtLehrer jwtLehrer)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Value.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfiguration.Value.Issuer,
            audience: _jwtConfiguration.Value.Audience,
            claims: jwtLehrer.GetClaims(),
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class JwtLehrer
{
    public required Guid Id { get; init; }
    public required string Benutzername { get; init; }
    public required string Nachname { get; init; }
    public required string Vorname { get; init; }
    
    public List<Claim> GetClaims()
    {
        return
        [
            new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
            new Claim(ClaimTypes.Name, Benutzername),
            new Claim(ClaimTypes.Surname, Nachname),
            new Claim(ClaimTypes.GivenName, Vorname),
        ];
    }

    public static JwtLehrer Parse(ClaimsPrincipal claimsPrincipal)
    {
        var idClaim = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
        var benutzernameClaim = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.Name);
        var nachnameClaim = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.Surname);
        var vornameClaim = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.GivenName);

        return new JwtLehrer
        {
            Id = Guid.Parse(idClaim.Value),
            Benutzername = benutzernameClaim.Value,
            Nachname = nachnameClaim.Value,
            Vorname = vornameClaim.Value
        };
    }
}
