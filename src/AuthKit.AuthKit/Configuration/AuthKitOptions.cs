using AuthKit.Core.Options;

namespace AuthKit.AuthKit.Configuration;

public class AuthKitOptions
{
    public string Name { get; set; } = "AuthKit";

    public DatabaseOptions Database { get; } = new();
    public JwtOptions Jwt { get; } = new();
    public RefreshTokenOptions RefreshTokens { get; } = new();
    public PasswordHashingOptions PasswordHashing { get; } = new();
    public RateLimitingOptions RateLimiting { get; } = new();
    public MultiTenancyOptions MultiTenancy { get; } = new();
    public TwoFactorOptions TwoFactor { get; } = new();
    public OAuthOptions OAuth { get; } = new();
    public ScalarOptions Scalar { get; } = new();
    public MigrationOptions Migration { get; } = new();
    public SeedOptions Seed { get; } = new();
}

public class AuthKitDbContextOptions
{
    public int? ConnectionTimeout { get; set; }
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public int? CommandTimeout { get; set; }
}
