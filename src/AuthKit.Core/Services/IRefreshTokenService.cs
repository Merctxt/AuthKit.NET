namespace AuthKit.Core.Services;

using AuthKit.Core.Models;
using AuthKit.Core.Services.DTOs;

public interface IRefreshTokenService
{
    Task<RefreshTokenRegistry> CreateAsync(Guid sessionId, CancellationToken ct = default);
    Task<bool> ValidateAsync(Guid jti, CancellationToken ct = default);
    Task<RefreshTokenRegistry> RotateAsync(Guid jti, Guid newSessionId, CancellationToken ct = default);
    Task<IResult> RevokeAsync(Guid jti, CancellationToken ct = default);
    Task<IResult> RevokeAllBySessionAsync(Guid sessionId, CancellationToken ct = default);
    Task<IResult> RevokeAllByUserAsync(Guid userId, CancellationToken ct = default);
}
