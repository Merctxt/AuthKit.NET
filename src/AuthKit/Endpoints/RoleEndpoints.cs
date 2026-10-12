namespace AuthKit.Endpoints;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class RoleEndpoints
{
    public static IEndpointConventionBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth/roles").WithTags("Roles");

        group.MapPost("/", async (IRoleService service, RoleRequest request) =>
        {
            return await service.CreateAsync(request);
        });

        group.MapGet("/", async (IRoleService service) =>
        {
            return await service.ListAsync();
        });

        group.MapGet("/{id:guid}", async (IRoleService service, Guid id) =>
        {
            return await service.GetByIdAsync(id);
        });

        group.MapPut("/{id:guid}", async (IRoleService service, Guid id, RoleRequest request) =>
        {
            return await service.UpdateAsync(id, request);
        });

        group.MapDelete("/{id:guid}", async (IRoleService service, Guid id) =>
        {
            return await service.DeleteAsync(id);
        });

        group.MapPost("/{userId:guid}/roles", async (IRoleService service, Guid userId, Guid roleId) =>
        {
            return await service.AssignRoleAsync(userId, roleId);
        });

        group.MapDelete("/{userId:guid}/roles/{roleId:guid}", async (IRoleService service, Guid userId, Guid roleId) =>
        {
            return await service.RemoveRoleAsync(userId, roleId);
        });

        group.MapGet("/{userId:guid}/roles", async (IRoleService service, Guid userId) =>
        {
            return await service.GetByUserIdAsync(userId);
        });

        return group;
    }
}
