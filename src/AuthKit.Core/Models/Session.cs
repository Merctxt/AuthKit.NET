namespace AuthKit.Core.Models;

public class Session
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DeviceInfo { get; set; } = string.Empty;
    public string RefreshTokenHash { get; set; } = string.Empty;
    public Guid RefreshTokenJti { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }

    public User User { get; set; } = null!;
}
