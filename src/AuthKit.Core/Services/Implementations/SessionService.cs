namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;

public class SessionService : ISessionService
{
    public Task<SessionDto> CreateAsync(CreateSessionRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(new SessionDto(Guid.NewGuid(), request.UserId, request.DeviceInfo, request.ExpiresAt, DateTime.UtcNow, false));
    }

    public Task<IResult> RevokeAsync(Guid sessionId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> RevokeByUserAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IEnumerable<SessionDto>> ListByUserAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<SessionDto>>(new List<SessionDto>());
    }

    public Task<bool> ValidateAsync(Guid sessionId, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }

    public Task<int> CountActiveByUserAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult(0);
    }

    public Task<IResult> TouchAsync(Guid sessionId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }
}
