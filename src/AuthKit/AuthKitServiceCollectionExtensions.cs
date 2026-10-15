namespace AuthKit;

using AuthKit.Configuration;
using AuthKit.Core.Services;
using AuthKit.Core.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class AuthKitServiceCollectionExtensions
{
    public static IServiceCollection AddAuthKit(this IServiceCollection services, Action<AuthKitOptions>? configure = null)
    {
        services.Configure(configure ?? (_ => { }));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<AuthKitOptions>>().Value);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRateLimitService, RateLimitService>();
        services.AddScoped<IUserManagerService, UserManagerService>();
        services.AddScoped<ILoginAttemptService, LoginAttemptService>();

        return services;
    }
}
