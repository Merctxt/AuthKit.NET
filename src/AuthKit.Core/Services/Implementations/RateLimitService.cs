namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;

public class RateLimitService : IRateLimitService
{
    public Task<RateLimitResult> CheckAsync(string identifier, string attemptType, CancellationToken ct = default)
    {
        return Task.FromResult(new RateLimitResult(false, 5, null));
    }

    public Task<IResult> RecordAttemptAsync(string identifier, string attemptType, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> UnblockAsync(string identifier, string attemptType, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }
}
