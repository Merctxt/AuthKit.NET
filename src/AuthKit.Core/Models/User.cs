namespace AuthKit.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public Tenant? Tenant { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<OAuthAccount> OAuthAccounts { get; set; } = new List<OAuthAccount>();
    public TwoFactorSecret? TwoFactorSecret { get; set; }
}
