namespace AuthKit.Core.Models;

public class RefreshTokenRegistry
{
    public Guid Jti { get; set; }
    public string ParentHash { get; set; } = string.Empty;
    public string ChildHash { get; set; } = string.Empty;
    public DateTime? UsedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Guid SessionId { get; set; }
}
