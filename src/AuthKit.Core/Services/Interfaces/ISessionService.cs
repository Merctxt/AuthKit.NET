namespace AuthKit.Core.Services;

using AuthKit.Core.Services.DTOs;

public interface ISessionService
{
    Task<SessionDto> CreateAsync(CreateSessionRequest request, CancellationToken ct = default);
    Task<IResult> RevokeAsync(Guid sessionId, CancellationToken ct = default);
    Task<IResult> RevokeByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IEnumerable<SessionDto>> ListByUserAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ValidateAsync(Guid sessionId, CancellationToken ct = default);
    Task<int> CountActiveByUserAsync(Guid userId, CancellationToken ct = default);
    Task<IResult> TouchAsync(Guid sessionId, CancellationToken ct = default);
}
