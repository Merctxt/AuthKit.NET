using Microsoft.EntityFrameworkCore;
using AuthKit.Core.Models;

namespace AuthKit.Data;

public class AuthKitDbContext : DbContext
{
    public AuthKitDbContext(DbContextOptions<AuthKitDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserClaim> UserClaims => Set<UserClaim>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<OAuthAccount> OAuthAccounts => Set<OAuthAccount>();
    public DbSet<TwoFactorSecret> TwoFactorSecrets => Set<TwoFactorSecret>();
    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();
    public DbSet<RefreshTokenRegistry> RefreshTokenRegistry => Set<RefreshTokenRegistry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Email, e.TenantId }).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.EmailConfirmed).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.LastLoginAt).IsRequired(false);

            entity.HasOne(e => e.Tenant).WithMany(e => e.Users).HasForeignKey(e => e.TenantId);
            entity.HasMany(e => e.UserRoles).WithOne(e => e.User).HasForeignKey(e => e.UserId);
            entity.HasMany(e => e.UserClaims).WithOne(e => e.User).HasForeignKey(e => e.UserId);
            entity.HasMany(e => e.Sessions).WithOne(e => e.User).HasForeignKey(e => e.UserId);
            entity.HasMany(e => e.OAuthAccounts).WithOne(e => e.User).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.TwoFactorSecret).WithOne(e => e.User).HasForeignKey<TwoFactorSecret>(e => e.UserId);
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Domain).HasMaxLength(200).IsRequired(false);
            entity.Property(e => e.Settings).HasDefaultValue("{}");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasIndex(e => e.Domain).IsUnique();

            entity.HasMany(e => e.Roles).WithOne().HasForeignKey(e => e.TenantId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Name, e.TenantId }).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasMany(e => e.UserRoles).WithOne(e => e.Role).HasForeignKey(e => e.RoleId);
            entity.HasMany(e => e.RolePermissions).WithOne(e => e.Role).HasForeignKey(e => e.RoleId);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId, e.TenantId });

            entity.HasOne(e => e.User).WithMany(e => e.UserRoles).HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.Role).WithMany(e => e.UserRoles).HasForeignKey(e => e.RoleId);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.Permission });

            entity.HasOne(e => e.Role).WithMany(e => e.RolePermissions).HasForeignKey(e => e.RoleId);
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClaimType).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ClaimValue).HasMaxLength(500);

            entity.HasOne(e => e.User).WithMany(e => e.UserClaims).HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DeviceInfo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.RefreshTokenHash).IsRequired();
            entity.Property(e => e.IsRevoked).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.ExpiresAt).IsRequired();

            entity.HasOne(e => e.User).WithMany(e => e.Sessions).HasForeignKey(e => e.UserId);

            entity.HasIndex(e => e.RefreshTokenJti).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpiresAt);
        });

        modelBuilder.Entity<OAuthAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Provider).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProviderId).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AccessToken).IsRequired();

            entity.HasOne(e => e.User).WithMany(e => e.OAuthAccounts).HasForeignKey(e => e.UserId);

            entity.HasIndex(e => new { e.UserId, e.Provider }).IsUnique();
        });

        modelBuilder.Entity<TwoFactorSecret>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.ProviderType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Secret).IsRequired().HasMaxLength(64);
            entity.Property(e => e.IsEnabled).HasDefaultValue(false);
            entity.Property(e => e.BackupCodesHashed).IsRequired().HasMaxLength(512);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<LoginAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Identifier).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AttemptType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.FailedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasIndex(e => new { e.Identifier, e.AttemptType, e.FailedAt });
        });

        modelBuilder.Entity<RefreshTokenRegistry>(entity =>
        {
            entity.HasKey(e => e.Jti);
            entity.Property(e => e.ParentHash).IsRequired();
            entity.Property(e => e.ChildHash).IsRequired();

            entity.HasOne<Session>().WithMany().HasForeignKey(e => e.SessionId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ParentHash);
            entity.HasIndex(e => e.ChildHash);
            entity.HasIndex(e => e.SessionId);
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var defaultTenantId = new Guid("00000000-0000-0000-0000-000000000001");
        var adminRoleId = new Guid("00000000-0000-0000-0000-000000000002");

        modelBuilder.Entity<Tenant>().HasData(
            new Tenant
            {
                Id = defaultTenantId,
                Name = "Default",
                Domain = "localhost",
                Settings = "{}",
                IsActive = true
            });

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRoleId,
                TenantId = defaultTenantId,
                Name = "Admin",
                Description = "Sistema Admin"
            });
    }
}

