using AuthKit.Core.Services;
using System.Security.Claims;

namespace AuthKit.Interfaces.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions, IEnumerable<string> claims);
    (string AccessToken, string RefreshToken) GenerateTokenPair(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions, IEnumerable<string> claims);
    ClaimsPrincipal? ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
    string? GetEmailFromToken(string token);
    IEnumerable<string>? GetRolesFromToken(string token);
}

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string token);
    Task SendPasswordResetEmailAsync(string email, string token);
    Task SendWelcomeEmailAsync(string email, string userName);
    Task SendTwoFactorCodeAsync(string email, string code);
}

public interface ISessionService
{
    Task CreateSessionAsync(Guid userId, string deviceInfo);
    Task<IEnumerable<SessionDto>> GetUserSessionsAsync(Guid userId);
    Task RevokeSessionAsync(Guid sessionId);
    Task RevokeAllUserSessionsAsync(Guid userId);
    Task InvalidateSessionAsync(Guid sessionId);
}

public interface IMfaService
{
    Task<string> GenerateSecretKeyAsync(Guid userId);
    Task<bool> VerifySecretKeyAsync(Guid userId, string secretKey, string code);
    Task EnableTwoFactorAsync(Guid userId, string secret, IEnumerable<string> backupCodes);
    Task DisableTwoFactorAsync(Guid userId);
    Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code);
    Task<bool> VerifyBackupCodeAsync(Guid userId, string code);
    Task<IEnumerable<string>> GenerateBackupCodesAsync(Guid userId);
}

public interface IAuthorizationService
{
    Task<bool> HasRoleAsync(Guid userId, string role, Guid tenantId);
    Task<bool> HasPermissionAsync(Guid userId, string permission, Guid tenantId);
    Task<bool> HasClaimAsync(Guid userId, string claimType, string claimValue);
    Task<IEnumerable<string>> GetRolesAsync(Guid userId, Guid tenantId);
    Task<IEnumerable<string>> GetPermissionsAsync(Guid userId, Guid tenantId);
    Task<IEnumerable<(string Type, string Value)>> GetClaimsAsync(Guid userId);
}

public interface ILoginService
{
    Task<LoginResult> LoginAsync(string email, string password, string? ipAddress = null);
    Task<LoginResult> LoginWithTwoFactorAsync(string email, string code);
    Task LogoutAsync(Guid userId);
}

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterAsync(string email, string username, string password, string? tenantId = null);
    Task<bool> ConfirmEmailAsync(string email, string token);
}

public interface IPasswordService
{
    Task<PasswordResult> ForgotPasswordAsync(string email);
    Task<PasswordResult> ResetPasswordAsync(string email, string token, string newPassword);
}

public interface ITenantService
{
    Task<TenantDto?> GetTenantAsync(Guid id);
    Task<TenantDto?> GetTenantByDomainAsync(string domain);
    Task<TenantDto> GetDefaultTenantAsync();
    Task<bool> UserBelongsToTenantAsync(Guid userId, Guid tenantId);
}

public interface IRateLimitingService
{
    bool IsRateLimited(string identifier, string endpoint);
    void RecordAttempt(string identifier, string endpoint, bool succeeded);
    int GetFailedAttempts(string identifier, string endpoint);
    void ClearAttempts(string identifier, string endpoint);
    int? GetRemainingAttempts(string identifier, string endpoint);
}
