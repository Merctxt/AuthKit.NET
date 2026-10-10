namespace AuthKit.Core.Services;

public record LoginResult(
    bool Success,
    string? AccessToken,
    string? RefreshToken,
    string? TwoFactorRequired,
    string? Message,
    List<string>? Roles = null,
    List<string>? Permissions = null,
    List<(string Type, string Value)>? Claims = null
);

public record RegistrationResult(
    bool Success,
    Guid? UserId,
    string? Message,
    bool RequiresEmailConfirmation
);

public record PasswordResult(
    bool Success,
    string? Message
);

public record SessionDto(
    Guid Id,
    string DeviceInfo,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    bool IsRevoked
);

public record TenantDto(
    Guid Id,
    string Name,
    string? Domain,
    bool IsActive
);
