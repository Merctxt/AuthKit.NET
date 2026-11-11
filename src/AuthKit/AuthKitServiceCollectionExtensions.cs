namespace AuthKit;

using AuthKit.Core.Configuration;
using AuthKit.Core.Services;
using AuthKit.Core.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class AuthKitServiceCollectionExtensions
{
    public static IServiceCollection AddAuthKit(this IServiceCollection services, Action<AuthKitOptions>? configure = null)
    {
        services.Configure(configure ?? (_ => { }));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<AuthKitOptions>>().Value);

        // Fase 1: Services com implementações reais
        services.AddScoped<IPasswordHasher, PasswordHasher>(sp =>
            new PasswordHasher(sp.GetRequiredService<IOptions<AuthKitOptions>>().Value.PasswordHash));

        services.AddScoped<IJwtService, JwtService>(sp =>
            new JwtService(sp.GetRequiredService<IOptions<AuthKitOptions>>().Value.Jwt));

        // Pending implementations — stubs to be completed in subsequent phases
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRateLimitService, RateLimitService>();
        services.AddScoped<IUserManagerService, UserManagerService>();
        services.AddScoped<ILoginAttemptService, LoginAttemptService>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();

        return services;
    }
}
