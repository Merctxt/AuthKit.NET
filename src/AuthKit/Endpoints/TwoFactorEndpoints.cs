namespace AuthKit.Endpoints;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class TwoFactorEndpoints
{
    public static IEndpointConventionBuilder MapTwoFactorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth/2fa").WithTags("2FA / TOTP");

        group.MapPost("/enable", async (ITwoFactorService service, Guid userId, EnableTwoFactorRequest request) =>
        {
            return await service.EnableAsync(userId, request);
        });

        group.MapPost("/disable", async (ITwoFactorService service, Guid userId, DisableTwoFactorRequest request) =>
        {
            return await service.DisableAsync(userId, request);
        });

        group.MapPost("/verify", async (ITwoFactorService service, Guid userId, VerifyTwoFactorRequest request) =>
        {
            return await service.VerifyCodeAsync(userId, request);
        });

        group.MapPost("/backups", async (ITwoFactorService service, Guid userId) =>
        {
            return await service.GenerateBackupCodesAsync(userId);
        });

        group.MapPost("/verify-backup", async (ITwoFactorService service, Guid userId, string code) =>
        {
            return await service.VerifyBackupCodeAsync(userId, code);
        });

        return group;
    }
}
