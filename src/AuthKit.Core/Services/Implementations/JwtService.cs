namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Configuration;
using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtService : IJwtService
{
    private readonly JwtOptions _options;

    public JwtService(JwtOptions options)
    {
        _options = options;
    }

    public Task<string> GenerateTokenAsync(Guid userId, List<string>? roles, CancellationToken ct = default)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (roles != null && roles.Any())
        {
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        }

        return Task.FromResult(GenerateJwt(claims));
    }

    private string GenerateJwt(IEnumerable<Claim> claims)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_options.SecretKey!);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, _options.SigningAlgorithm);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = DateTime.UtcNow.Add(_options.Expiration),
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(tokenDescriptor);
    }

    public Task<string> GenerateRefreshTokenAsync(CancellationToken ct = default)
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Task.FromResult(Convert.ToBase64String(bytes));
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var parts = token.Split('.');
        if (parts.Length != 3)
            return false;

        try
        {
            var handler = new JsonWebTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_options.SecretKey!);
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = !string.IsNullOrEmpty(_options.Issuer),
                ValidIssuer = _options.Issuer,
                ValidateAudience = !string.IsNullOrEmpty(_options.Audience),
                ValidAudience = _options.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var result = await handler.ValidateTokenAsync(token, validationParams);
            return result?.ClaimsIdentity != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<TokenResponse> RotateTokenAsync(string token, string refreshToken, CancellationToken ct = default)
    {
        var principal = await ParseTokenAsync(token, ct);
        if (principal == null)
        {
            throw new SecurityTokenException("Invalid access token.");
        }

        var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new SecurityTokenException("No user ID in token."));

        var roles = principal.FindAll(ClaimTypes.Role)
            .Select(c => c.Value!)
            .ToList();

        // Generate a new token with a fresh JTI (claim) so it's always different
        var newAccessToken = await GenerateTokenAsync(userId, roles, ct);
        var newRefreshToken = await GenerateRefreshTokenAsync(ct);

        return new TokenResponse(newAccessToken, newRefreshToken, DateTime.UtcNow.Add(_options.Expiration));
    }

    public async Task<IResult> RegenerateTokenAsync(Guid userId, CancellationToken ct = default)
    {
        var newToken = await GenerateTokenAsync(userId, new List<string>(), ct);
        var newRefreshToken = await GenerateRefreshTokenAsync(ct);

        return new Result(true, "Token regenerated.", null);
    }

    public async Task<ClaimsPrincipal?> ParseTokenAsync(string token, CancellationToken ct = default)
    {
        try
        {
            var handler = new JsonWebTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_options.SecretKey!);
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = !string.IsNullOrEmpty(_options.Issuer),
                ValidIssuer = _options.Issuer,
                ValidateAudience = !string.IsNullOrEmpty(_options.Audience),
                ValidAudience = _options.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var result = await handler.ValidateTokenAsync(token, validationParams);
            if (result?.ClaimsIdentity != null)
            {
                return new ClaimsPrincipal(result.ClaimsIdentity);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
