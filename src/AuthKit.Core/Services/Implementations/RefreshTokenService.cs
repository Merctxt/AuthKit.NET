namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Configuration;
using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;
using AuthKit.Core;
using AuthKit.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AuthDbContext _db;
    private readonly RefreshTokenOptions _options;
    private readonly IJwtService _jwt;

    public RefreshTokenService(AuthDbContext db, RefreshTokenOptions options, IJwtService jwt)
    {
        _db = db;
        _options = options;
        _jwt = jwt;
    }
    public Task<string> CreateAsync(Guid sessionId, CancellationToken ct = default)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<IResult> ValidateAsync(string token, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<TokenResponse> RotateAsync(string token, CancellationToken ct = default)
    {
        return Task.FromResult(new TokenResponse(string.Empty, string.Empty, DateTime.UtcNow));
    }

    public Task<IResult> RevokeAsync(string token, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> RevokeAllBySessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> RevokeAllByUserAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }
}
