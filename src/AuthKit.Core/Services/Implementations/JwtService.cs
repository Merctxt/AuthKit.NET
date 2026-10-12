namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using System.Security.Claims;

public class JwtService : IJwtService
{
    public Task<string> GenerateTokenAsync(Guid userId, List<string> roles, CancellationToken ct = default)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<string> GenerateRefreshTokenAsync(CancellationToken ct = default)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }

    public Task<TokenResponse> RotateTokenAsync(string token, string refreshToken, CancellationToken ct = default)
    {
        return Task.FromResult(new TokenResponse(string.Empty, string.Empty, DateTime.UtcNow));
    }

    public Task<IResult> RegenerateTokenAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<ClaimsPrincipal?> ParseTokenAsync(string token, CancellationToken ct = default)
    {
        return Task.FromResult<ClaimsPrincipal?>(null);
    }
}
