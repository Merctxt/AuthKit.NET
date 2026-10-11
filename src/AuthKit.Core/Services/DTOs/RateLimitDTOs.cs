namespace AuthKit.Core.Services.DTOs;

public record RateLimitResult(bool IsBlocked, int RemainingAttempts, TimeSpan? LockoutRemaining);
