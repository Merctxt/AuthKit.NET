namespace AuthKit.Core.Options;

public enum HashingAlgorithm
{
    Argon2id,
    BCrypt
}

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string? ProviderName { get; set; }
    public bool AutoMigrate { get; set; } = true;
    public bool AutoSeed { get; set; } = true;
    public string? DbContextClassName { get; set; } = "AuthKitDbContext";
    public int? CommandTimeout { get; set; }
    public bool EnableSensitiveDataLogging { get; set; } = false;
}

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public TimeSpan AccessExpiration { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan RefreshExpiration { get; set; } = TimeSpan.FromDays(7);
    public string Issuer { get; set; } = "AuthKit.NET";
    public string? Audience { get; set; }
    public int ClockSkewMinutes { get; set; } = 1;
    public bool RequireExpirationTime { get; set; } = true;
    public string SigningAlgorithm { get; set; } = "HS256";
}

public class RefreshTokenOptions
{
    public bool EnableRotation { get; set; } = true;
    public int MaxReuseCount { get; set; } = 1;
    public TimeSpan AbsoluteExpiry { get; set; } = TimeSpan.FromDays(30);
    public TimeSpan SlidingExpiry { get; set; } = TimeSpan.FromDays(7);
    public bool RevokeOtherTokensOnReuse { get; set; } = true;
    public bool RevokeAllOnSuspiciousReuse { get; set; } = true;
}

public class PasswordHashingOptions
{
    public HashingAlgorithm Algorithm { get; set; } = HashingAlgorithm.Argon2id;
    public int Argon2MemoryKB { get; set; } = 65536;
    public int Argon2Iterations { get; set; } = 3;
    public int Argon2Parallelism { get; set; } = 1;
    public int BCryptStrength { get; set; } = 12;
}

public class RateLimitingOptions
{
    public bool Enabled { get; set; } = true;
    public int LoginMaxAttempts { get; set; } = 5;
    public TimeSpan LoginWindow { get; set; } = TimeSpan.FromMinutes(5);
    public int LoginByAccountMaxAttempts { get; set; } = 10;
    public TimeSpan LoginByAccountWindow { get; set; } = TimeSpan.FromMinutes(10);
    public int ForgotPasswordMaxAttempts { get; set; } = 3;
    public TimeSpan ForgotPasswordWindow { get; set; } = TimeSpan.FromMinutes(15);
    public int RegisterMaxAttempts { get; set; } = 3;
    public TimeSpan RegisterWindow { get; set; } = TimeSpan.FromMinutes(15);
    public bool StoreInDatabase { get; set; } = true;
}

public class MultiTenancyOptions
{
    public bool Enabled { get; set; } = false;
    public bool RequireTenant { get; set; } = true;
    public string DefaultTenantId { get; set; } = "default";
    public bool DefaultTenantExists { get; set; } = false;
}

public class TwoFactorOptions
{
    public bool Enabled { get; set; } = false;
    public int TotpDigits { get; set; } = 6;
    public TimeSpan TotpPeriod { get; set; } = TimeSpan.FromSeconds(30);
    public int TotpEntropy { get; set; } = 20;
    public int BackupCodesCount { get; set; } = 10;
    public int BackupCodesLength { get; set; } = 8;
}

public class ScalarOptions
{
    public bool Enabled { get; set; } = true;
    public string RoutePrefix { get; set; } = "/auth/docs";
    public string Theme { get; set; } = "dark";
    public bool ShowContentType { get; set; } = true;
    public bool ShowExtensions { get; set; } = true;
    public bool ShowJsonEditor { get; set; } = true;
    public bool HideTryItOut { get; set; } = false;
    public int DefaultModelsExpandDepth { get; set; } = 2;
    public int DefaultModelExpandDepth { get; set; } = 1;
    public bool DefaultModelRendering { get; set; } = true;
    public bool DisplayOperationId { get; set; } = true;
    public bool DisplayRequestDuration { get; set; } = true;
    public bool DocExpansion { get; set; } = true;
    public bool Filter { get; set; } = false;
    public bool ShowCommonExtensions { get; set; } = true;
    public bool SortEndpoints { get; set; } = false;
    public string? CustomCssUrl { get; set; }
    public string? CustomJsUrl { get; set; }
}

public class MigrationOptions
{
    public bool ApplyOnStartup { get; set; } = true;
    public bool EnforceForeignKeys { get; set; } = true;
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 500;
}

public class SeedOptions
{
    public bool EnableDefaultSeed { get; set; } = true;
    public bool CreateDefaultAdmin { get; set; } = true;
    public string DefaultAdminEmail { get; set; } = "admin@example.com";
    public string DefaultAdminPassword { get; set; } = "Ch@ngeM3!Admin";
    public string DefaultAdminRole { get; set; } = "Admin";
    public string? TenantName { get; set; } = "Default";
}

public class OAuthProviderOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string? Scopes { get; set; }
    public string? RedirectUri { get; set; }
    public string? UserInfoEndpoint { get; set; }
    public string? AuthorizationEndpoint { get; set; }
    public string? TokenEndpoint { get; set; }
    public string? ClaimMappingEmail { get; set; } = "email";
    public string? ClaimMappingName { get; set; } = "name";
    public string? ClaimMappingId { get; set; } = "sub";
    public bool AutoCreateUser { get; set; } = true;
}

public class OAuthOptions
{
    public bool Enabled { get; set; } = false;
    public Dictionary<string, OAuthProviderOptions> Providers { get; set; } = new();
    public string? DefaultProvider { get; set; }
    public TimeSpan TokenExpiry { get; set; } = TimeSpan.FromHours(1);
    public bool LinkExistingUsers { get; set; } = true;
}
