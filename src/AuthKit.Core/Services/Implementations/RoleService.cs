namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using AuthKit.Core;
using AuthKit.Data.Models;
using Microsoft.EntityFrameworkCore;

public class RoleService : IRoleService
{
    private readonly AuthDbContext _db;

    public RoleService(AuthDbContext db)
    {
        _db = db;
    }
    public Task<RoleDto> CreateAsync(RoleRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(new RoleDto(Guid.NewGuid(), request.Name, request.Description, request.Permissions ?? new List<string>()));
    }

    public Task<IEnumerable<RoleDto>> ListAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<RoleDto>>(new List<RoleDto>());
    }

    public Task<RoleDto?> GetByIdAsync(Guid roleId, CancellationToken ct = default)
    {
        return Task.FromResult<RoleDto?>(null);
    }

    public Task<Role?> GetByNameAsync(string roleName, CancellationToken ct = default)
    {
        return Task.FromResult<Role?>(null);
    }

    public Task<IResult> UpdateAsync(Guid roleId, RoleRequest request, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> DeleteAsync(Guid roleId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> AssignRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }

    public Task<IEnumerable<RoleDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<RoleDto>>(new List<RoleDto>());
    }
}
