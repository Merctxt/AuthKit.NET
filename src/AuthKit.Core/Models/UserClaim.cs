namespace AuthKit.Core.Models;

public class UserClaim
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}
