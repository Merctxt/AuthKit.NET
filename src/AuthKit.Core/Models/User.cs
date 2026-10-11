namespace AuthKit.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public bool IsLockedOut { get; set; }
    public DateTime? LockedOutUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<UserTwoFactorSecret> TwoFactorSecrets { get; set; } = new List<UserTwoFactorSecret>();
}
