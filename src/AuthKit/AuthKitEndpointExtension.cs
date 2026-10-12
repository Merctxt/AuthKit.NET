namespace AuthKit;

using AuthKit.Configuration;
using AuthKit.Core.Services;
using AuthKit.Endpoints;
using AuthKit.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class AuthKitEndpointExtension
{
    public static WebApplication UseAuthKit(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();

        var options = app.Services.GetRequiredService<IOptions<AuthKitOptions>>().Value;
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
