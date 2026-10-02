using AuthKit.Core.Models;

namespace AuthKit.Data.Abstractions;

public interface IAuthRepository
{
    // Users
    Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsersAsync(Guid tenantId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> UserExistsByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> UserExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);

    // Tenants
    Task<Tenant?> GetTenantByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tenant?> GetTenantByDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<Tenant?> GetDefaultTenantAsync(CancellationToken cancellationToken = default);
    Task CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);

    // Roles
    Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleByNameAsync(string name, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetRolesByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task CreateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);

    // UserRoles
    Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task RemoveUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<bool> HasRoleAsync(Guid userId, string role, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    // RolePermissions
    Task AddRolePermissionAsync(RolePermission rolePermission, CancellationToken cancellationToken = default);
    Task RemoveRolePermissionAsync(Guid roleId, string permission, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionAsync(Guid roleId, string permission, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    // UserClaims (ABAC)
    Task CreateUserClaimAsync(UserClaim claim, CancellationToken cancellationToken = default);
    Task RemoveUserClaimAsync(Guid claimId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserClaim>> GetUserClaimsAsync(Guid userId, CancellationToken cancellationToken = default);

    // Sessions
    Task CreateSessionAsync(Session session, CancellationToken cancellationToken = default);
    Task<Session?> GetSessionAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>> GetUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeSessionAsync(Guid id, CancellationToken cancellationToken = default);
    Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteSessionAsync(Guid id, CancellationToken cancellationToken = default);

    // OAuth Accounts
    Task CreateOAuthAccountAsync(OAuthAccount account, CancellationToken cancellationToken = default);
    Task<OAuthAccount?> GetOAuthAccountAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OAuthAccount?> GetOAuthAccountByProviderAsync(Guid userId, string provider, CancellationToken cancellationToken = default);
    Task UpdateOAuthAccountAsync(OAuthAccount account, CancellationToken cancellationToken = default);
    Task DeleteOAuthAccountAsync(Guid id, CancellationToken cancellationToken = default);

    // Two-Factor Auth
    Task CreateOrUpdateTwoFactorSecretAsync(TwoFactorSecret secret, CancellationToken cancellationToken = default);
    Task<TwoFactorSecret?> GetTwoFactorSecretAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteTwoFactorSecretAsync(Guid userId, CancellationToken cancellationToken = default);

    // Refresh Token Registry
    Task CreateRefreshTokenAsync(RefreshTokenRegistry registry, CancellationToken cancellationToken = default);
    Task<RefreshTokenRegistry?> GetRefreshTokenByJtiAsync(Guid jti, CancellationToken cancellationToken = default);
    Task MarkTokenUsedAsync(Guid jti, Guid childJti, CancellationToken cancellationToken = default);
    Task MarkTokenRevokedAsync(Guid jti, CancellationToken cancellationToken = default);
    Task RevokeTokensBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task DeleteExpiredTokensAsync(DateTime before, CancellationToken cancellationToken = default);

    // Login Attempts (Rate Limiting)
    Task CreateLoginAttemptAsync(LoginAttempt attempt, CancellationToken cancellationToken = default);
    Task<int> GetFailedAttemptsCountAsync(string identifier, string attemptType, CancellationToken cancellationToken = default);
    Task ClearLoginAttemptsAsync(string identifier, CancellationToken cancellationToken = default);
}
