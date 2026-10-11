namespace AuthKit.Core.Services;

using AuthKit.Core.Services.DTOs;

public interface IRateLimitService
{
    Task<RateLimitResult> CheckAsync(string identifier, string attemptType, CancellationToken ct = default);
    Task<IResult> RecordAttemptAsync(string identifier, string attemptType, CancellationToken ct = default);
    Task<IResult> UnblockAsync(string identifier, string attemptType, CancellationToken ct = default);
}
