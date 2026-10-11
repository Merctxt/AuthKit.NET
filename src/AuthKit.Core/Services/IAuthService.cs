namespace AuthKit.Core.Services;

using AuthKit.Core.Services.DTOs;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(LoginRequest request, string? ipAddress = null, CancellationToken ct = default);
    Task<IResult> LogoutAsync(Guid userId, CancellationToken ct = default);
    Task<IResult> RefreshEmailAsync(Guid userId, CancellationToken ct = default);
    Task<IResult> VerifyEmailAsync(Guid userId, string code, CancellationToken ct = default);
    Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken ct = default);
    Task<IResult> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default);
    Task<IResult> LogoutAllAsync(Guid userId, CancellationToken ct = default);
    Task<IResult> ForgotPasswordAsync(string email, CancellationToken ct = default);
    Task<IResult> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken ct = default);
}
