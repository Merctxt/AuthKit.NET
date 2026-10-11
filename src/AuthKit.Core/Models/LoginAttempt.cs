using System.ComponentModel.DataAnnotations;

namespace AuthKit.Core.Models;

public class LoginAttempt
{
    public Guid Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string AttemptType { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    public bool Succeeded { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }
}
