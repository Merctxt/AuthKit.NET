namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;

public class AuthService : IAuthService
{
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
