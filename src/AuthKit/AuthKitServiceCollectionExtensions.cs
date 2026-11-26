namespace AuthKit;

using AuthKit.Core;
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

        // Fase 0: DbContext injetado em todos os serviços
        services.AddScoped<IPasswordHasher, PasswordHasher>(sp =>
            new PasswordHasher(sp.GetRequiredService<IOptions<AuthKitOptions>>().Value.PasswordHash));

        services.AddScoped<IJwtService, JwtService>(sp =>
            new JwtService(sp.GetRequiredService<IOptions<AuthKitOptions>>().Value.Jwt));

        services.AddScoped<IUserManagerService, UserManagerService>(sp =>
            new UserManagerService(
                sp.GetRequiredService<AuthDbContext>(),
                sp.GetRequiredService<IOptions<AuthKitOptions>>().Value));

        services.AddScoped<IRoleService, RoleService>(sp =>
            new RoleService(sp.GetRequiredService<AuthDbContext>()));

        services.AddScoped<ITwoFactorService, TwoFactorService>(sp =>
            new TwoFactorService(
                sp.GetRequiredService<AuthDbContext>(),
                sp.GetRequiredService<IOptions<AuthKitOptions>>().Value.TwoFactor));

        services.AddScoped<ISessionService, SessionService>(sp =>
            new SessionService(
                sp.GetRequiredService<AuthDbContext>(),
                sp.GetRequiredService<IOptions<AuthKitOptions>>().Value.Session));

        services.AddScoped<IRefreshTokenService, RefreshTokenService>(sp =>
            new RefreshTokenService(
                sp.GetRequiredService<AuthDbContext>(),
                sp.GetRequiredService<IOptions<AuthKitOptions>>().Value.RefreshToken,
                sp.GetRequiredService<IJwtService>()));

        services.AddScoped<IRateLimitService, RateLimitService>(sp =>
            new RateLimitService(
                sp.GetRequiredService<AuthDbContext>(),
                sp.GetRequiredService<IOptions<AuthKitOptions>>().Value.RateLimit));

        services.AddScoped<ILoginAttemptService, LoginAttemptService>(sp =>
            new LoginAttemptService(
                sp.GetRequiredService<AuthDbContext>(),
                sp.GetRequiredService<IOptions<AuthKitOptions>>().Value.AccountLockout));

        services.AddScoped<IAuthService, AuthService>(sp =>
            new AuthService(
                sp.GetRequiredService<AuthDbContext>(),
                sp.GetRequiredService<IPasswordHasher>(),
                sp.GetRequiredService<IJwtService>(),
                sp.GetRequiredService<IRefreshTokenService>(),
                sp.GetRequiredService<ISessionService>(),
                sp.GetRequiredService<ILoginAttemptService>(),
                sp.GetRequiredService<ITwoFactorService>(),
                sp.GetRequiredService<IRateLimitService>(),
                sp.GetRequiredService<IOptions<AuthKitOptions>>().Value));

        return services;
    }
}
