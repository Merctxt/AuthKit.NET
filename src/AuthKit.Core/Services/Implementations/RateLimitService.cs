namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Configuration;
using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using AuthKit.Core;
using AuthKit.Data.Models;
using Microsoft.EntityFrameworkCore;

public class RateLimitService : IRateLimitService
{
    private readonly AuthDbContext _db;
    private readonly RateLimitOptions _options;

    public RateLimitService(AuthDbContext db, RateLimitOptions options)
    {
        _db = db;
        _options = options;
    }
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
