namespace AuthKit.Core.Services;

using AuthKit.Core.Models;
using AuthKit.Core.Services.DTOs;

public interface ILoginAttemptService
{
    Task<LoginAttempt> RecordAsync(LoginAttemptRequest request, CancellationToken ct = default);
    Task<IEnumerable<LoginAttemptDto>> GetByIdentifierAsync(string identifier, CancellationToken ct = default);
    Task<bool> IsBlockedAsync(string identifier, string attemptType, CancellationToken ct = default);
    Task<IResult> ClearExpiredAsync(DateTime before, CancellationToken ct = default);
}
