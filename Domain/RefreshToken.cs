using Auth_Jwt.Infrastructure;

namespace Auth_Jwt.Domain;
public class RefreshToken : Entity
{
    public RefreshToken()
    {
    }

    public RefreshToken(
        string token,
        DateTime expires,
        User user)
    {
        Token = token;
        Expires = expires;
        User = user;
    }

    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public User User { get; set; }
    public bool Active => DateTime.UtcNow <= Expires;
}

