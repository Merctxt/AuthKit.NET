namespace AuthKit.Core.Services;

using AuthKit.Core.Services.DTOs;
using System.Security.Claims;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(Guid userId, List<string> roles, CancellationToken ct = default);
    Task<string> GenerateRefreshTokenAsync(CancellationToken ct = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default);
    Task<TokenResponse> RotateTokenAsync(string token, string refreshToken, CancellationToken ct = default);
    Task<IResult> RegenerateTokenAsync(Guid userId, CancellationToken ct = default);
    Task<ClaimsPrincipal?> ParseTokenAsync(string token, CancellationToken ct = default);
}
