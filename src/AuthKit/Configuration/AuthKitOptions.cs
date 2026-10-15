namespace AuthKit.Configuration;

public class AuthKitOptions
{
    public JwtOptions Jwt { get; set; } = new();
    public RefreshTokenOptions RefreshToken { get; set; } = new();
    public PasswordHashOptions PasswordHash { get; set; } = new();
    public PasswordPolicyOptions PasswordPolicy { get; set; } = new();
    public TwoFactorOptions TwoFactor { get; set; } = new();
    public RateLimitOptions RateLimit { get; set; } = new();
    public AccountLockoutOptions AccountLockout { get; set; } = new();
    public EmailOptions Email { get; set; } = new();
    public SessionOptions Session { get; set; } = new();
    public SmtpOptions Smtp { get; set; } = new();
    public string EndpointsPrefix { get; set; } = "/v1";
    public OpenTelemetryOptions? OpenTelemetry { get; set; }
    public UserValidationOptions UserValidation { get; set; } = new();
    public PaginationOptions Pagination { get; set; } = new();
}

public class JwtOptions
{
    public string? SecretKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string SigningAlgorithm { get; set; } = "HS256";
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(15);
}

public class RefreshTokenOptions
{
    public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(7);
    public TimeSpan? AbsoluteExpiration { get; set; } = TimeSpan.FromDays(30);
    public bool Rotation { get; set; } = true;
    public int ReuseDetection { get; set; } = 1;
}

public class PasswordHashOptions
{
    public int Memory { get; set; } = 65536;
    public int Iterations { get; set; } = 3;
    public int Parallelism { get; set; } = 1;
}

public class PasswordPolicyOptions
{
    public int MinLength { get; set; } = 8;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireNonAlphanumeric { get; set; } = true;
    public int MaxRepeatedCharacters { get; set; } = 3;
}

public class TwoFactorOptions
{
    public bool Enabled { get; set; }
    public int Digits { get; set; } = 6;
    public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(30);
    public int BackupCodesCount { get; set; } = 8;
    public string TOTPIssuer { get; set; } = "AuthKit";
}

public class RateLimitOptions
{
    public bool Enabled { get; set; }
    public int MaxAttempts { get; set; } = 5;
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
}

public class AccountLockoutOptions
{
    public bool Enabled { get; set; } = true;
    public int MaxFailedAttempts { get; set; } = 5;
    public TimeSpan DefaultLockoutTime { get; set; } = TimeSpan.FromMinutes(15);
}

public class SessionOptions
{
    public int MaxActivePerUser { get; set; } = 5;
    public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(1);
}

public class SmtpOptions
{
    public bool Enabled { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool UseSsl { get; set; } = true;
    public string? FromEmail { get; set; }
    public string? FromName { get; set; }
    public int ConnectionTimeout { get; set; } = 30000;
}

public class EmailOptions
{
    public int MaxChanges { get; set; } = 3;
    public TimeSpan Window { get; set; } = TimeSpan.FromDays(30);
}

public class OpenTelemetryOptions
{
    public bool Enabled { get; set; }
    public string? ServiceName { get; set; } = "AuthKit";
    public string? ServiceVersion { get; set; } = "1.0.0";
    public string? Endpoint { get; set; }
}

public class UserValidationOptions
{
    public bool RequireUniqueUsername { get; set; } = false;
    public int UsernameMinLength { get; set; } = 3;
    public int UsernameMaxLength { get; set; } = 50;
    public string UsernamePattern { get; set; } = @"^[a-zA-Z0-9_]+$";
}

public class PaginationOptions
{
    public int DefaultPageSize { get; set; } = 20;
    public int MaxPageSize { get; set; } = 100;
}
