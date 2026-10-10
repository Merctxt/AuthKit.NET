using AuthKit.Core.Models;
using AuthKit.Core.Options;

namespace AuthKit.Core.Tests.Models;

public class UserTests
{
    [Fact]
    public void User_CanBeCreated_WithValidData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        // Act
        var user = new User
        {
            Id = userId,
            TenantId = tenantId,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = "$2a$12$hashed",
            EmailConfirmed = true
        };

        // Assert
        Assert.Equal(userId, user.Id);
        Assert.Equal(tenantId, user.TenantId);
        Assert.Equal("test@example.com", user.Email);
        Assert.NotNull(user.CreatedAt);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact]
    public void User_DefaultCreatedAt_IsDefault()
    {
        // Arrange
        var user = new User();

        // Assert
        Assert.Equal(DateTime.MinValue, user.CreatedAt);
        Assert.Equal(DateTime.MinValue, user.UpdatedAt);
    }
}

public class TenantTests
{
    [Fact]
    public void Tenant_CanBeCreated_WithValidData()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        // Act
        var tenant = new Tenant
        {
            Id = tenantId,
            Name = "Test Tenant",
            Domain = "test.example.com",
            IsActive = true
        };

        // Assert
        Assert.Equal(tenantId, tenant.Id);
        Assert.Equal("Test Tenant", tenant.Name);
        Assert.Equal("test.example.com", tenant.Domain);
        Assert.True(tenant.IsActive);
    }

    [Fact]
    public void Tenant_DefaultIsActive_IsFalse()
    {
        // Arrange
        var tenant = new Tenant();

        // Assert
        Assert.False(tenant.IsActive);
    }
}

public class RoleTests
{
    [Fact]
    public void Role_CanBeCreated_WithValidData()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        // Act
        var role = new Role
        {
            Id = roleId,
            TenantId = tenantId,
            Name = "Admin",
            Description = "Administrator role"
        };

        // Assert
        Assert.Equal(roleId, role.Id);
        Assert.Equal("Admin", role.Name);
        Assert.Equal("Administrator role", role.Description);
    }
}

public class UserRoleTests
{
    [Fact]
    public void UserRole_CanBeCreated_WithCompositeKeys()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        // Act
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            TenantId = tenantId
        };

        // Assert
        Assert.Equal(userId, userRole.UserId);
        Assert.Equal(roleId, userRole.RoleId);
        Assert.Equal(tenantId, userRole.TenantId);
    }
}

public class RolePermissionTests
{
    [Fact]
    public void RolePermission_CanBeCreated_WithCompositeKey()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        var rolePermission = new RolePermission
        {
            RoleId = roleId,
            Permission = "users:manage"
        };

        // Assert
        Assert.Equal(roleId, rolePermission.RoleId);
        Assert.Equal("users:manage", rolePermission.Permission);
    }
}

public class SessionTests
{
    [Fact]
    public void Session_CanBeCreated_WithValidData()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var session = new Session
        {
            Id = sessionId,
            UserId = userId,
            DeviceInfo = "Chrome on Windows 11",
            RefreshTokenHash = "sha256hash",
            RefreshTokenJti = Guid.NewGuid(),
            ExpiresAt = now.AddHours(1),
            IsRevoked = false
        };

        // Assert
        Assert.Equal(sessionId, session.Id);
        Assert.Equal(userId, session.UserId);
        Assert.False(session.IsRevoked);
    }
}

public class TwoFactorSecretTests
{
    [Fact]
    public void TwoFactorSecret_Defaults_IsEnabledFalse()
    {
        // Arrange & Act
        var secret = new TwoFactorSecret();

        // Assert
        Assert.False(secret.IsEnabled);
    }
}

public class LoginAttemptTests
{
    [Fact]
    public void LoginAttempt_CanBeCreated_WithValidData()
    {
        // Arrange
        var attemptId = Guid.NewGuid();

        // Act
        var attempt = new LoginAttempt
        {
            Id = attemptId,
            Identifier = "test@example.com",
            AttemptType = "Login",
            IpAddress = "192.168.1.1",
            Succeeded = false
        };

        // Assert
        Assert.Equal(attemptId, attempt.Id);
        Assert.Equal("test@example.com", attempt.Identifier);
        Assert.Equal("Login", attempt.AttemptType);
        Assert.False(attempt.Succeeded);
    }
}

public class RefreshTokenRegistryTests
{
    [Fact]
    public void RefreshTokenRegistry_CanBeCreated_WithJti()
    {
        // Arrange
        var jti = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        // Act
        var registry = new RefreshTokenRegistry
        {
            Jti = jti,
            ParentHash = "parent_hash",
            ChildHash = "child_hash",
            SessionId = sessionId
        };

        // Assert
        Assert.Equal(jti, registry.Jti);
        Assert.Equal("parent_hash", registry.ParentHash);
        Assert.Equal("child_hash", registry.ChildHash);
        Assert.Equal(sessionId, registry.SessionId);
    }
}

public class OAuthAccountTests
{
    [Fact]
    public void OAuthAccount_CanBeCreated_WithValidData()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var account = new OAuthAccount
        {
            Id = accountId,
            UserId = userId,
            Provider = "Google",
            ProviderId = "google-user-123",
            AccessToken = "encrypted_token",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        // Assert
        Assert.Equal(accountId, account.Id);
        Assert.Equal("Google", account.Provider);
        Assert.Equal("google-user-123", account.ProviderId);
    }
}

public class UserClaimTests
{
    [Fact]
    public void UserClaim_CanBeCreated_WithValidData()
    {
        // Arrange
        var claimId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var claim = new UserClaim
        {
            Id = claimId,
            UserId = userId,
            ClaimType = "department",
            ClaimValue = "Engineering"
        };

        // Assert
        Assert.Equal(claimId, claim.Id);
        Assert.Equal("department", claim.ClaimType);
        Assert.Equal("Engineering", claim.ClaimValue);
    }
}
