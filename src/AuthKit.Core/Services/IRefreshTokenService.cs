namespace AuthKit.Core.Services;

using AuthKit.Core.Services.DTOs;

public interface IRefreshTokenService
{
    Task<string> CreateAsync(Guid sessionId, CancellationToken ct = default);
    Task<IResult> ValidateAsync(string token, CancellationToken ct = default);
    Task<TokenResponse> RotateAsync(string token, CancellationToken ct = default);
    Task<IResult> RevokeAsync(string token, CancellationToken ct = default);
    Task<IResult> RevokeAllBySessionAsync(Guid sessionId, CancellationToken ct = default);
    Task<IResult> RevokeAllByUserAsync(Guid userId, CancellationToken ct = default);
}
