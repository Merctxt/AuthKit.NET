namespace AuthKit.Core.Services;

using AuthKit.Core.Services.DTOs;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(LoginRequest request, string? ipAddress = null, CancellationToken ct = default);
    Task<IResult> LogoutAsync(Guid userId, CancellationToken ct = default);
    Task<IResult> VerifyEmailAsync(EmailVerificationRequest request, CancellationToken ct = default);
    Task<IResult> ResendVerificationEmailAsync(string email, CancellationToken ct = default);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
    Task<IResult> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default);
    Task<IResult> LogoutAllAsync(Guid userId, CancellationToken ct = default);
    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default);
    Task<IResult> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
}
