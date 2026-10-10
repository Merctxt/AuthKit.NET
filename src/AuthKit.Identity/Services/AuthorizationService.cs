using AuthKit.Core.Services;
using AuthKit.Interfaces.Repositories;

namespace AuthKit.Identity.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IAuthRepository _repository;

    public AuthorizationService(IAuthRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HasRoleAsync(Guid userId, string role, Guid tenantId)
    {
        return await _repository.HasRoleAsync(userId, role, tenantId);
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission, Guid tenantId)
    {
        var roles = await _repository.GetUserRolesAsync(userId);
        foreach (var role in roles)
        {
            if (role.TenantId == tenantId && await _repository.HasPermissionAsync(role.Id, permission))
                return true;
        }
        return false;
    }

    public async Task<bool> HasClaimAsync(Guid userId, string claimType, string claimValue)
    {
        var claims = await _repository.GetUserClaimsAsync(userId);
        return claims.Any(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
    }

    public async Task<IEnumerable<string>> GetRolesAsync(Guid userId, Guid tenantId)
    {
        var roles = await _repository.GetUserRolesAsync(userId);
        return roles.Where(r => r.TenantId == tenantId).Select(r => r.Name);
    }

    public async Task<IEnumerable<string>> GetPermissionsAsync(Guid userId, Guid tenantId)
    {
        var roles = await _repository.GetUserRolesAsync(userId);
        var permissions = new HashSet<string>();

        foreach (var role in roles.Where(r => r.TenantId == tenantId))
        {
            var rolePermissions = await _repository.GetRolePermissionsAsync(role.Id);
            foreach (var perm in rolePermissions)
                permissions.Add(perm);
        }

        return permissions;
    }

    public async Task<IEnumerable<(string Type, string Value)>> GetClaimsAsync(Guid userId)
    {
        var claims = await _repository.GetUserClaimsAsync(userId);
        return claims.Select(c => (c.ClaimType, c.ClaimValue));
    }
}
