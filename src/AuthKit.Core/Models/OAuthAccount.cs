namespace AuthKit.Core.Models;

public class OAuthAccount
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }

    public User User { get; set; } = null!;
}
