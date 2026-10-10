using AuthKit.Core.Options;
using AuthKit.Core.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthKit.AuthKit.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenService(JwtOptions jwtOptions)
    {
        _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
    }

    public string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions, IEnumerable<string> claims)
    {
        var claimsList = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new("sub", userId.ToString())
        };

        if (roles?.Any() == true)
        {
            claimsList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        if (permissions?.Any() == true)
        {
            claimsList.AddRange(permissions.Select(claim => new Claim("permission", claim)));
        }

        if (claims?.Any() == true)
        {
            foreach (var claim in claims)
            {
                var parts = claim.Split(':', 2);
                if (parts.Length == 2)
                {
                    claimsList.Add(new Claim(parts[0], parts[1]));
                }
                else
                {
                    claimsList.Add(new Claim("custom", claim));
                }
            }
        }

        return GenerateToken(claimsList);
    }

    public (string AccessToken, string RefreshToken) GenerateTokenPair(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions, IEnumerable<string> claims)
    {
        var accessToken = GenerateAccessToken(userId, email, roles, permissions, claims);
        var refreshToken = GenerateRefreshToken();

        return (accessToken, refreshToken);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = !string.IsNullOrEmpty(_jwtOptions.Audience),
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(_jwtOptions.ClockSkewMinutes),
                RequireExpirationTime = _jwtOptions.RequireExpirationTime
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        if (principal == null) return null;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) 
            ?? principal.FindFirst("sub");

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    public string? GetEmailFromToken(string token)
    {
        var principal = ValidateToken(token);
        return principal?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public IEnumerable<string>? GetRolesFromToken(string token)
    {
        var principal = ValidateToken(token);
        return principal?.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }

    private string GenerateToken(List<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_jwtOptions.AccessExpiration),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience ?? _jwtOptions.Issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                _jwtOptions.SigningAlgorithm)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
