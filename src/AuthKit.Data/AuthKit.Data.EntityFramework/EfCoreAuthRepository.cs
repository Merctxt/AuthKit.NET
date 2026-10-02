using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using AuthKit.Core.Models;
using AuthKit.Data.Abstractions;

namespace AuthKit.Data.EntityFramework;

public class EfCoreAuthRepository : IAuthRepository
{
    private readonly AuthKitDbContext _context;

    public EfCoreAuthRepository(AuthKitDbContext context)
    {
        _context = context;
    }

    // Users
    public async Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserClaims)
            .Include(u => u.OAuthAccounts)
            .Include(u => u.TwoFactorSecret)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetUserByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserClaims)
            .Include(u => u.OAuthAccounts)
            .Include(u => u.TwoFactorSecret)
            .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId, cancellationToken);

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserClaims)
            .Include(u => u.OAuthAccounts)
            .Include(u => u.TwoFactorSecret)
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public async Task<IEnumerable<User>> GetUsersAsync(Guid tenantId, int page, int pageSize, CancellationToken cancellationToken = default) =>
        await _context.Users
            .Where(u => u.TenantId == tenantId)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserClaims)
            .OrderBy(u => u.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<bool> UserExistsByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.Users.AnyAsync(u => u.Email == email && u.TenantId == tenantId, cancellationToken);

    public async Task<bool> UserExistsByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await _context.Users.AnyAsync(u => u.Username == username, cancellationToken);

    public async Task CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // Tenants
    public async Task<Tenant?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Tenants.FindAsync(new object[] { id }, cancellationToken);

    public async Task<Tenant?> GetTenantByDomainAsync(string domain, CancellationToken cancellationToken = default) =>
        await _context.Tenants.FirstOrDefaultAsync(t => t.Domain == domain, cancellationToken);

    public async Task<Tenant?> GetDefaultTenantAsync(CancellationToken cancellationToken = default) =>
        await _context.Tenants.FirstOrDefaultAsync(cancellationToken);

    public async Task CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(tenant, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // Roles
    public async Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Roles
            .Include(r => r.UserRoles)
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<Role?> GetRoleByNameAsync(string name, Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.Roles.FirstOrDefaultAsync(r => r.Name == name && r.TenantId == tenantId, cancellationToken);

    public async Task<IEnumerable<Role>> GetRolesByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.Roles.Where(r => r.TenantId == tenantId).ToListAsync(cancellationToken);

    public async Task CreateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _context.Roles.AddAsync(role, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.FindAsync(new object[] { id }, cancellationToken);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // UserRoles
    public async Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        await _context.UserRoles.AddAsync(userRole, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        if (userRole != null)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> HasRoleAsync(Guid userId, string role, Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.TenantId == tenantId && ur.Role.Name == role, cancellationToken);

    public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync(cancellationToken);

    // RolePermissions
    public async Task AddRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default)
    {
        await _context.RolePermissions.AddAsync(rolePermission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRolePermissionAsync(Guid roleId, string permission, CancellationToken cancellationToken = default)
    {
        var rolePermission = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.Permission == permission, cancellationToken);
        if (rolePermission != null)
        {
            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> HasPermissionAsync(Guid roleId, string permission, CancellationToken cancellationToken = default) =>
        await _context.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.Permission == permission, cancellationToken);

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default) =>
        await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission)
            .ToListAsync(cancellationToken);

    // UserClaims (ABAC)
    public async Task CreateUserClaimAsync(UserClaim claim, CancellationToken cancellationToken = default)
    {
        await _context.UserClaims.AddAsync(claim, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveUserClaimAsync(Guid claimId, CancellationToken cancellationToken = default)
    {
        var claim = await _context.UserClaims.FindAsync(new object[] { claimId }, cancellationToken);
        if (claim != null)
        {
            _context.UserClaims.Remove(claim);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<UserClaim>> GetUserClaimsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.UserClaims.Where(uc => uc.UserId == userId).ToListAsync(cancellationToken);

    // Sessions
    public async Task CreateSessionAsync(Session session, CancellationToken cancellationToken = default)
    {
        session.CreatedAt = DateTime.UtcNow;
        await _context.Sessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Session?> GetSessionAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Sessions.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<Session>> GetUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.Sessions.Where(s => s.UserId == userId).ToListAsync(cancellationToken);

    public async Task RevokeSessionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await _context.Sessions.FindAsync(new object[] { id }, cancellationToken);
        if (session != null)
        {
            session.IsRevoked = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var sessions = await _context.Sessions.Where(s => s.UserId == userId).ToListAsync(cancellationToken);
        foreach (var session in sessions)
        {
            session.IsRevoked = true;
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteSessionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await _context.Sessions.FindAsync(new object[] { id }, cancellationToken);
        if (session != null)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // OAuth Accounts
    public async Task CreateOAuthAccountAsync(OAuthAccount account, CancellationToken cancellationToken = default)
    {
        await _context.OAuthAccounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<OAuthAccount?> GetOAuthAccountAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.OAuthAccounts.FindAsync(new object[] { id }, cancellationToken);

    public async Task<OAuthAccount?> GetOAuthAccountByProviderAsync(Guid userId, string provider, CancellationToken cancellationToken = default) =>
        await _context.OAuthAccounts.FirstOrDefaultAsync(oa => oa.UserId == userId && oa.Provider == provider, cancellationToken);

    public async Task UpdateOAuthAccountAsync(OAuthAccount account, CancellationToken cancellationToken = default)
    {
        _context.OAuthAccounts.Update(account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOAuthAccountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var account = await _context.OAuthAccounts.FindAsync(new object[] { id }, cancellationToken);
        if (account != null)
        {
            _context.OAuthAccounts.Remove(account);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // Two-Factor Auth
    public async Task CreateOrUpdateTwoFactorSecretAsync(TwoFactorSecret secret, CancellationToken cancellationToken = default)
    {
        var existing = await _context.TwoFactorSecrets.FindAsync(new object[] { secret.UserId }, cancellationToken);
        if (existing == null)
        {
            secret.CreatedAt = DateTime.UtcNow;
            await _context.TwoFactorSecrets.AddAsync(secret, cancellationToken);
        }
        else
        {
            _context.TwoFactorSecrets.Update(secret);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TwoFactorSecret?> GetTwoFactorSecretAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _context.TwoFactorSecrets.FindAsync(new object[] { userId }, cancellationToken);

    public async Task DeleteTwoFactorSecretAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var secret = await _context.TwoFactorSecrets.FindAsync(new object[] { userId }, cancellationToken);
        if (secret != null)
        {
            _context.TwoFactorSecrets.Remove(secret);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // Refresh Token Registry
    public async Task CreateRefreshTokenAsync(RefreshTokenRegistry registry, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokenRegistry.AddAsync(registry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshTokenRegistry?> GetRefreshTokenByJtiAsync(Guid jti, CancellationToken cancellationToken = default) =>
        await _context.RefreshTokenRegistry.FindAsync(new object[] { jti }, cancellationToken);

    public async Task MarkTokenUsedAsync(Guid jti, Guid childJti, CancellationToken cancellationToken = default)
    {
        var registry = await _context.RefreshTokenRegistry.FindAsync(new object[] { jti }, cancellationToken);
        if (registry != null)
        {
            registry.UsedAt = DateTime.UtcNow;
            registry.ChildHash = HashToken(childJti);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkTokenRevokedAsync(Guid jti, CancellationToken cancellationToken = default)
    {
        var registry = await _context.RefreshTokenRegistry.FindAsync(new object[] { jti }, cancellationToken);
        if (registry != null)
        {
            registry.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RevokeTokensBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokenRegistry.Where(r => r.SessionId == sessionId).ToListAsync(cancellationToken);
        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteExpiredTokensAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        var expired = await _context.RefreshTokenRegistry.Where(r => r.ExpiresAt < before).ToListAsync(cancellationToken);
        _context.RefreshTokenRegistry.RemoveRange(expired);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // Login Attempts (Rate Limiting)
    public async Task CreateLoginAttemptAsync(LoginAttempt attempt, CancellationToken cancellationToken = default)
    {
        attempt.FailedAt = DateTime.UtcNow;
        await _context.LoginAttempts.AddAsync(attempt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetFailedAttemptsCountAsync(string identifier, string attemptType, CancellationToken cancellationToken = default)
    {
        var window = DateTime.UtcNow.AddMinutes(-5);
        return await _context.LoginAttempts
            .CountAsync(a => a.Identifier == identifier && a.AttemptType == attemptType && !a.Succeeded && a.FailedAt > window, cancellationToken);
    }

    public async Task ClearLoginAttemptsAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var attempts = await _context.LoginAttempts.Where(a => a.Identifier == identifier).ToListAsync(cancellationToken);
        _context.LoginAttempts.RemoveRange(attempts);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private string HashToken(Guid jti)
    {
        return Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(jti.ToString())));
    }
}
