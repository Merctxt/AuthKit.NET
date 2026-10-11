namespace AuthKit.Core.Services;

using AuthKit.Core.Models;
using AuthKit.Core.Services.DTOs;

public interface IRoleService
{
    Task<RoleDto> CreateAsync(RoleRequest request, CancellationToken ct = default);
    Task<IEnumerable<RoleDto>> ListAsync(CancellationToken ct = default);
    Task<RoleDto?> GetByIdAsync(Guid roleId, CancellationToken ct = default);
    Task<Role?> GetByNameAsync(string roleName, CancellationToken ct = default);
    Task<IResult> UpdateAsync(Guid roleId, RoleRequest request, CancellationToken ct = default);
    Task<IResult> DeleteAsync(Guid roleId, CancellationToken ct = default);
    Task<IResult> AssignRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default);
    Task<IResult> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct = default);
    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken ct = default);
    Task<IEnumerable<RoleDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
}
