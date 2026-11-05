// Built with help from Claude
using System.Security.Claims;
using System.Text;
using AgriTrack.Api.Dtos;
using AgriTrack.Api.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AgriTrack.Api.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public AuthResponse CreateToken(ApplicationUser user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiresMinutes"));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        // The claims are the data stored inside the token
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.FullName)
            }),
            Expires = expires,
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return new AuthResponse(token, expires, user.Email ?? "", user.FullName);
    }
}