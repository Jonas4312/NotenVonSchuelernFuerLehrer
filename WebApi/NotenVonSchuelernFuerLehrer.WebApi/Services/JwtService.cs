using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotenVonSchuelernFuerLehrer.Domain.Model;
using NotenVonSchuelernFuerLehrer.WebApi.Configuration;

namespace NotenVonSchuelernFuerLehrer.WebApi.Services;

public class JwtService
{
    private readonly IOptions<JwtConfiguration> _jwtConfiguration;

    public JwtService(IOptions<JwtConfiguration> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration;
    }

    public string GenerateToken(Lehrer lehrer)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Value.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfiguration.Value.Issuer,
            audience: _jwtConfiguration.Value.Audience,
            claims: GetClaims(lehrer),
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private List<Claim> GetClaims(Lehrer lehrer)
    {
        return
        [
            new Claim(ClaimTypes.NameIdentifier, lehrer.Id.ToString()),
            new Claim(ClaimTypes.Name, lehrer.Benutzername),
            new Claim(ClaimTypes.Surname, lehrer.Nachname),
            new Claim(ClaimTypes.GivenName, lehrer.Vorname),
        ];
    }
}
