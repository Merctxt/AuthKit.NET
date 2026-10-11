namespace AuthKit.Core.Services.DTOs;

public record RegisterRequest(string Email, string Password, string? Username = null);

public record LoginRequest(string Identifier, string Password);

public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, string TokenType = "Bearer");

public record RefreshTokenRequest(string Token);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Email, string Token, string NewPassword);

public record EmailVerificationRequest(string Email, string Code);

public record UserDto(Guid Id, string Email, string? Username, bool EmailConfirmed, bool IsLockedOut, DateTime? LockedOutUntil, DateTime CreatedAt, DateTime? LastLoginAt);

public record AuthResult(bool Success, string Message, UserDto? User = null, TokenResponse? Token = null);
