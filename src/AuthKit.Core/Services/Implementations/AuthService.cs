namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Configuration;
using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using AuthKit.Core;
using AuthKit.Data.Models;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;
    private readonly IRefreshTokenService _refreshToken;
    private readonly ISessionService _session;
    private readonly ILoginAttemptService _attempts;
    private readonly ITwoFactorService _twoFactor;
    private readonly IRateLimitService _rateLimit;
    private readonly AuthKitOptions _options;

    public AuthService(
        AuthDbContext db,
        IPasswordHasher hasher,
        IJwtService jwt,
        IRefreshTokenService refreshToken,
        ISessionService session,
        ILoginAttemptService attempts,
        ITwoFactorService twoFactor,
        IRateLimitService rateLimit,
        AuthKitOptions options)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
        _refreshToken = refreshToken;
        _session = session;
        _attempts = attempts;
        _twoFactor = twoFactor;
        _rateLimit = rateLimit;
        _options = options;
    }
    public Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(new AuthResult(false, "Not implemented.", null, null));
    }

    public Task<AuthResult> LoginAsync(LoginRequest request, string? ipAddress = null, CancellationToken ct = default)
    {
        return Task.FromResult(new AuthResult(false, "Not implemented.", null, null));
    }

    public Task<IResult> LogoutAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> VerifyEmailAsync(EmailVerificationRequest request, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> ResendVerificationEmailAsync(string email, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(new TokenResponse(string.Empty, string.Empty, DateTime.UtcNow));
    }

    public Task<IResult> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> LogoutAllAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }
}
