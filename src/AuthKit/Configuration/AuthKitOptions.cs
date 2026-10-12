namespace AuthKit.Configuration;

public class AuthKitOptions
{
    public JwtOptions Jwt { get; set; } = new();
    public RefreshTokenOptions RefreshToken { get; set; } = new();
    public PasswordHashOptions PasswordHash { get; set; } = new();
    public TwoFactorOptions TwoFactor { get; set; } = new();
    public RateLimitOptions RateLimit { get; set; } = new();
    public EmailOptions Email { get; set; } = new();
    public SessionOptions Session { get; set; } = new();
    public string EndpointsPrefix { get; set; } = "/v1";
}

public class JwtOptions
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(15);
}

public class RefreshTokenOptions
{
    public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(7);
    public bool Rotation { get; set; } = true;
    public int ReuseDetection { get; set; } = 1;
}

public class PasswordHashOptions
{
    public int Memory { get; set; } = 65536;
    public int Iterations { get; set; } = 3;
    public int Parallelism { get; set; } = 1;
}

public class TwoFactorOptions
{
    public bool Enabled { get; set; }
    public int Digits { get; set; } = 6;
    public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(30);
    public int BackupCodesCount { get; set; } = 8;
}

public class RateLimitOptions
{
    public bool Enabled { get; set; }
    public int MaxAttempts { get; set; } = 5;
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(5);
}

public class SessionOptions
{
    public int MaxActivePerUser { get; set; } = 5;
}

public class EmailOptions
{
    public int MaxChanges { get; set; } = 3;
    public TimeSpan Window { get; set; } = TimeSpan.FromDays(30);
}
