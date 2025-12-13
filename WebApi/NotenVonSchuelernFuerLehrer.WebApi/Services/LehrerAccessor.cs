namespace NotenVonSchuelernFuerLehrer.WebApi.Services;

public class LehrerAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LehrerAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public JwtLehrer ErmittleLehrerJwt()
    {
        var currentUser = _httpContextAccessor.HttpContext?.User;
        ArgumentNullException.ThrowIfNull(currentUser);
        return JwtLehrer.Parse(currentUser);
    }
}