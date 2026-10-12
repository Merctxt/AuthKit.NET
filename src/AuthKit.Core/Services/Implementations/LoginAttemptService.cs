namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Models;
using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;

public class LoginAttemptService : ILoginAttemptService
{
    public Task<Guid> RecordAsync(LoginAttemptRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(Guid.NewGuid());
    }

    public Task<IEnumerable<LoginAttemptDto>> GetByIdentifierAsync(string identifier, CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<LoginAttemptDto>>(new List<LoginAttemptDto>());
    }

    public Task<bool> IsBlockedAsync(string identifier, string attemptType, CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }

    public Task<IResult> ClearExpiredAsync(DateTime before, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }
}
