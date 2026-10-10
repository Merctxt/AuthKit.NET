using AuthKit.Core.Services;
using AuthKit.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthKit.Identity;

public static class AuthKitIdentityServiceCollectionExtensions
{
    public static IServiceCollection AddAuthKitIdentity(this IServiceCollection services)
    {
        // Identity Services
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IMfaService, MfaService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
