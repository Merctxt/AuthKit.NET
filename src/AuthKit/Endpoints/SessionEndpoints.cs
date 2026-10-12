namespace AuthKit.Endpoints;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class SessionEndpoints
{
    public static IEndpointConventionBuilder MapSessionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth/sessions").WithTags("Sessions");

        group.MapGet("/", async (ISessionService service, Guid userId) =>
        {
            return await service.ListByUserAsync(userId);
        });

        group.MapDelete("/{id:guid}", async (ISessionService service, Guid sessionId) =>
        {
            return await service.RevokeAsync(sessionId);
        });

        group.MapDelete("/all", async (ISessionService service, Guid userId) =>
        {
            return await service.RevokeByUserAsync(userId);
        });

        return group;
    }
}
