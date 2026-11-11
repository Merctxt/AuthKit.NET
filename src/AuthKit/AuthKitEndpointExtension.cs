namespace AuthKit;

using AuthKit.Core.Configuration;
using AuthKit.Core.Services;
using AuthKit.Endpoints;
using AuthKit.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public static class AuthKitEndpointExtension
{
    private static readonly Type _loggerType = typeof(AuthKitEndpointExtension);

    public static WebApplication UseAuthKit(this WebApplication app)
    {
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(_loggerType);

        var options = app.Services.GetRequiredService<IOptions<AuthKitOptions>>().Value;
        var errors = ConfigurationValidation.ValidateAll(options, logger);

        if (errors.Count > 0)
        {
            var message = string.Join("; ", errors);
            throw new InvalidOperationException($"AuthKit configuration validation failed: {message}");
        }

        app.UseMiddleware<GlobalExceptionMiddleware>();

        var prefix = options.EndpointsPrefix;

        var authGroup = app.MapGroup(prefix);

        authGroup.MapAuthEndpoints();
        authGroup.MapTwoFactorEndpoints();
        authGroup.MapSessionEndpoints();
        authGroup.MapUserManagementEndpoints();
        authGroup.MapRoleEndpoints();

        app.MapGet($"{prefix}/health", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }));

        return app;
    }
}
