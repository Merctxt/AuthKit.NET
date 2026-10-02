namespace AuthKit.Core.Models;

public class LoginAttempt
{
    public Guid Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string AttemptType { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
    public bool Succeeded { get; set; }
}
