using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ContractTesting.Api;

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token);

public static class AuthService
{
    public const string Issuer = "ContractTesting.Api";
    public const string Audience = "contract-testing-app";
    public const string SecretKey = "just-an-example-secret-key-not-a-real-one";

    public static string? ValidateAndGenerateToken(string email, string password)
    {
        // I know this is not the way, but only for demo purposes
        if (email == "admin@example.com" && password == "admin123")
        {
            return GenerateToken("f47ac10b-58cc-4372-a567-0e02b2c3d479", email);
        }

        return null;
    }

    private static string GenerateToken(string id, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Sub, id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
