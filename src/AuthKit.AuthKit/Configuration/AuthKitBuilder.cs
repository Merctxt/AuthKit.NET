using AuthKit.Core.Options;

namespace AuthKit.AuthKit.Configuration;

public class AuthKitBuilder
{
    private readonly AuthKitOptions _options;

    public AuthKitBuilder(AuthKitOptions options)
    {
        _options = options;
    }

    public AuthKitOptions Options => _options;

    public AuthKitBuilder SetConnectionString(string connectionString, string? providerName = null)
    {
        _options.Database.ConnectionString = connectionString;
        _options.Database.ProviderName = providerName;
        return this;
    }

    public AuthKitBuilder UseDatabase(string connectionString, string? providerName = null, bool autoMigrate = true)
    {
        _options.Database.ConnectionString = connectionString;
        _options.Database.ProviderName = providerName;
        _options.Database.AutoMigrate = autoMigrate;
        _options.Database.AutoSeed = true;
        return this;
    }

    public AuthKitBuilder ConfigureDbContext(Action<AuthKitDbContextOptions> configure)
    {
        var ctxOptions = new AuthKitDbContextOptions();
        configure(ctxOptions);
        _options.Database.EnableSensitiveDataLogging = ctxOptions.EnableSensitiveDataLogging;
        _options.Database.CommandTimeout = ctxOptions.CommandTimeout;
        return this;
    }

    public AuthKitBuilder UseJwtBearer(string secret, TimeSpan? accessExpiration = null, string? issuer = null, string? audience = null)
    {
        _options.Jwt.Secret = secret;
        if (accessExpiration.HasValue) _options.Jwt.AccessExpiration = accessExpiration.Value;
        if (issuer != null) _options.Jwt.Issuer = issuer;
        if (audience != null) _options.Jwt.Audience = audience;
        return this;
    }

    public AuthKitBuilder ConfigureJwt(Action<JwtOptions> configure)
    {
        configure(_options.Jwt);
        return this;
    }

    public AuthKitBuilder UseRefreshTokens(TimeSpan? absoluteExpiry = null, TimeSpan? slidingExpiry = null, bool rotation = true)
    {
        if (absoluteExpiry.HasValue) _options.RefreshTokens.AbsoluteExpiry = absoluteExpiry.Value;
        if (slidingExpiry.HasValue) _options.RefreshTokens.SlidingExpiry = slidingExpiry.Value;
        _options.RefreshTokens.EnableRotation = rotation;
        return this;
    }

    public AuthKitBuilder ConfigureRefreshTokens(Action<RefreshTokenOptions> configure)
    {
        configure(_options.RefreshTokens);
        return this;
    }

    public AuthKitBuilder SetPasswordHashing(HashingAlgorithm algorithm, int? memoryKB = null, int? iterations = null, int? parallelism = null, int? bcryptStrength = null)
    {
        _options.PasswordHashing.Algorithm = algorithm;
        if (memoryKB.HasValue) _options.PasswordHashing.Argon2MemoryKB = memoryKB.Value;
        if (iterations.HasValue) _options.PasswordHashing.Argon2Iterations = iterations.Value;
        if (parallelism.HasValue) _options.PasswordHashing.Argon2Parallelism = parallelism.Value;
        if (bcryptStrength.HasValue) _options.PasswordHashing.BCryptStrength = bcryptStrength.Value;
        return this;
    }

    public AuthKitBuilder ConfigurePasswordHashing(Action<PasswordHashingOptions> configure)
    {
        configure(_options.PasswordHashing);
        return this;
    }

    public AuthKitBuilder EnableRateLimiting(int loginMaxAttempts = 5, TimeSpan? loginWindow = null, int loginByAccountMaxAttempts = 10, TimeSpan? loginByAccountWindow = null, int? forgotPasswordMaxAttempts = null, TimeSpan? forgotPasswordWindow = null, int? registerMaxAttempts = null, TimeSpan? registerWindow = null)
    {
        _options.RateLimiting.Enabled = true;
        _options.RateLimiting.LoginMaxAttempts = loginMaxAttempts;
        if (loginWindow.HasValue) _options.RateLimiting.LoginWindow = loginWindow.Value;
        _options.RateLimiting.LoginByAccountMaxAttempts = loginByAccountMaxAttempts;
        if (loginByAccountWindow.HasValue) _options.RateLimiting.LoginByAccountWindow = loginByAccountWindow.Value;
        if (forgotPasswordMaxAttempts.HasValue) _options.RateLimiting.ForgotPasswordMaxAttempts = forgotPasswordMaxAttempts.Value;
        if (forgotPasswordWindow.HasValue) _options.RateLimiting.ForgotPasswordWindow = forgotPasswordWindow.Value;
        if (registerMaxAttempts.HasValue) _options.RateLimiting.RegisterMaxAttempts = registerMaxAttempts.Value;
        if (registerWindow.HasValue) _options.RateLimiting.RegisterWindow = registerWindow.Value;
        return this;
    }

    public AuthKitBuilder ConfigureRateLimiting(Action<RateLimitingOptions> configure)
    {
        configure(_options.RateLimiting);
        return this;
    }

    public AuthKitBuilder EnableMultiTenancy(Action<MultiTenancyOptions>? configure = null, params Func<string, bool>[] tenantResolvers)
    {
        _options.MultiTenancy.Enabled = true;
        _options.MultiTenancy.RequireTenant = true;
        if (configure != null) configure(_options.MultiTenancy);
        return this;
    }

    public AuthKitBuilder ConfigureMultiTenancy(Action<MultiTenancyOptions> configure)
    {
        configure(_options.MultiTenancy);
        return this;
    }

    public AuthKitBuilder EnableTwoFactor(Action<TwoFactorOptions>? configure = null)
    {
        _options.TwoFactor.Enabled = true;
        if (configure != null) configure(_options.TwoFactor);
        return this;
    }

    public AuthKitBuilder ConfigureTwoFactor(Action<TwoFactorOptions> configure)
    {
        configure(_options.TwoFactor);
        return this;
    }

    public AuthKitBuilder EnableOAuthProviders(Action<OAuthProvidersBuilder> configure)
    {
        _options.OAuth.Enabled = true;
        var builder = new OAuthProvidersBuilder(_options.OAuth.Providers);
        configure(builder);
        if (_options.OAuth.Providers.Count > 0 && string.IsNullOrEmpty(_options.OAuth.DefaultProvider))
        {
            _options.OAuth.DefaultProvider = _options.OAuth.Providers.Keys.First();
        }
        return this;
    }

    public AuthKitBuilder ConfigureOAuth(Action<OAuthOptions> configure)
    {
        configure(_options.OAuth);
        return this;
    }

    public AuthKitBuilder AddOAuthProvider(string name, string clientId, string clientSecret, Action<OAuthProviderOptions>? configure = null)
    {
        var provider = new OAuthProviderOptions
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        };
        configure?.Invoke(provider);
        _options.OAuth.Providers[name] = provider;
        if (string.IsNullOrEmpty(_options.OAuth.DefaultProvider))
        {
            _options.OAuth.DefaultProvider = name;
        }
        return this;
    }

    public AuthKitBuilder EnableScalarUI(string? routePrefix = null, string? theme = null, Action<ScalarOptions>? configure = null)
    {
        _options.Scalar.Enabled = true;
        if (routePrefix != null) _options.Scalar.RoutePrefix = routePrefix;
        if (theme != null) _options.Scalar.Theme = theme;
        if (configure != null) configure(_options.Scalar);
        return this;
    }

    public AuthKitBuilder ConfigureScalar(Action<ScalarOptions> configure)
    {
        configure(_options.Scalar);
        return this;
    }

    public AuthKitBuilder ConfigureMigration(Action<MigrationOptions> configure)
    {
        configure(_options.Migration);
        return this;
    }

    public AuthKitBuilder ConfigureSeed(Action<SeedOptions> configure)
    {
        configure(_options.Seed);
        return this;
    }

    public AuthKitBuilder DisableAutoMigrate()
    {
        _options.Database.AutoMigrate = false;
        _options.Migration.ApplyOnStartup = false;
        return this;
    }

    public AuthKitBuilder DisableAutoSeed()
    {
        _options.Database.AutoSeed = false;
        _options.Seed.EnableDefaultSeed = false;
        return this;
    }
}

public class OAuthProvidersBuilder
{
    private readonly Dictionary<string, OAuthProviderOptions> _providers;

    public OAuthProvidersBuilder(Dictionary<string, OAuthProviderOptions> providers)
    {
        _providers = providers;
    }

    public OAuthProvidersBuilder AddGoogle(string clientId, string clientSecret, Action<OAuthProviderOptions>? configure = null)
    {
        return AddProvider("Google", clientId, clientSecret, configure);
    }

    public OAuthProvidersBuilder AddGitHub(string clientId, string clientSecret, Action<OAuthProviderOptions>? configure = null)
    {
        return AddProvider("GitHub", clientId, clientSecret, configure);
    }

    public OAuthProvidersBuilder AddMicrosoft(string clientId, string clientSecret, Action<OAuthProviderOptions>? configure = null)
    {
        return AddProvider("Microsoft", clientId, clientSecret, configure);
    }

    public OAuthProvidersBuilder AddFacebook(string appId, string appSecret, Action<OAuthProviderOptions>? configure = null)
    {
        return AddProvider("Facebook", appId, appSecret, configure);
    }

    public OAuthProvidersBuilder AddProvider(string name, string clientId, string clientSecret, Action<OAuthProviderOptions>? configure = null)
    {
        var provider = new OAuthProviderOptions
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        };
        configure?.Invoke(provider);
        _providers[name] = provider;
        return this;
    }
}
