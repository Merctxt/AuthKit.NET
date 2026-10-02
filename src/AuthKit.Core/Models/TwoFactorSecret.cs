namespace AuthKit.Core.Models;

public class TwoFactorSecret
{
    public Guid UserId { get; set; }

    public string ProviderType { get; set; } = "TOTP";
    public string Secret { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string BackupCodesHashed { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
