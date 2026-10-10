using Microsoft.Extensions.Options;
using AuthKit.Core.Options;
using AuthKit.Core.Services;
using AuthKit.Interfaces.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace AuthKit.Identity.Services;

public class SessionService : ISessionService
{
    private readonly IAuthRepository _repository;
    private readonly RefreshTokenOptions _refreshTokenOptions;

    public SessionService(
        IAuthRepository repository,
        IOptions<RefreshTokenOptions> refreshTokenOptions)
    {
        _repository = repository;
        _refreshTokenOptions = refreshTokenOptions.Value;
    }

    public async Task CreateSessionAsync(Guid userId, string deviceInfo)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var tokenHash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        var jti = Guid.NewGuid();

        var session = new Core.Models.Session
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DeviceInfo = deviceInfo,
            RefreshTokenHash = Convert.ToBase64String(tokenHash),
            RefreshTokenJti = jti,
            ExpiresAt = DateTime.UtcNow.Add(_refreshTokenOptions.AbsoluteExpiry),
            IsRevoked = false
        };

        await _repository.CreateSessionAsync(session);

        // Register refresh token
        await _repository.CreateRefreshTokenAsync(new Core.Models.RefreshTokenRegistry
        {
            Jti = jti,
            ParentHash = Convert.ToBase64String(tokenHash),
            SessionId = session.Id
        });
    }

    public async Task<IEnumerable<SessionDto>> GetUserSessionsAsync(Guid userId)
    {
        var sessions = await _repository.GetUserSessionsAsync(userId);
        return sessions.Select(s => new SessionDto(
            s.Id,
            s.DeviceInfo,
            s.CreatedAt,
            s.ExpiresAt,
            s.IsRevoked));
    }

    public async Task RevokeSessionAsync(Guid sessionId)
    {
        await _repository.RevokeSessionAsync(sessionId);
    }

    public async Task RevokeAllUserSessionsAsync(Guid userId)
    {
        await _repository.RevokeAllUserSessionsAsync(userId);
    }

    public async Task InvalidateSessionAsync(Guid sessionId)
    {
        var session = await _repository.GetSessionAsync(sessionId);
        if (session != null)
        {
            await _repository.RevokeSessionAsync(sessionId);
        }
    }
}
