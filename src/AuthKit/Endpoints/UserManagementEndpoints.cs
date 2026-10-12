namespace AuthKit.Endpoints;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class UserManagementEndpoints
{
    public static IEndpointConventionBuilder MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth/users").WithTags("User Management");

        group.MapGet("/", async (IUserManagerService service, [AsParameters] PaginationQuery query) =>
        {
            return await service.ListAsync(query.Page, query.PageSize);
        });

        group.MapGet("/{id:guid}", async (IUserManagerService service, Guid id) =>
        {
            return await service.GetByIdAsync(id);
        });

        group.MapPut("/me", async (IUserManagerService service, Guid userId, UpdateProfileRequest request) =>
        {
            return await service.UpdateAsync(userId, request);
        });

        group.MapPut("/me/email", async (IUserManagerService service, Guid userId, ChangeEmailRequest request) =>
        {
            return await service.ChangeEmailAsync(userId, request);
        });

        group.MapPost("/{id:guid}/lock", async (IUserManagerService service, Guid id) =>
        {
            return await service.LockAsync(id);
        });

        group.MapPost("/{id:guid}/unlock", async (IUserManagerService service, Guid id) =>
        {
            return await service.UnlockAsync(id);
        });

        group.MapDelete("/{id:guid}", async (IUserManagerService service, Guid id) =>
        {
            return await service.DeleteAsync(id);
        });

        return group;
    }

    public record PaginationQuery(int Page = 1, int PageSize = 20);
}
