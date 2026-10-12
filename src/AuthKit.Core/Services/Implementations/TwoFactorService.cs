namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;

public class TwoFactorService : ITwoFactorService
{
    public Task<TwoFactorResponse> EnableAsync(Guid userId, EnableTwoFactorRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(new TwoFactorResponse(false, "Not implemented."));
    }

    public Task<IResult> DisableAsync(Guid userId, DisableTwoFactorRequest request, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> VerifyCodeAsync(Guid userId, VerifyTwoFactorRequest request, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<TwoFactorResponse> GenerateBackupCodesAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult(new TwoFactorResponse(false, "Not implemented.", BackupCodes: new List<string>()));
    }

    public Task<IResult> VerifyBackupCodeAsync(Guid userId, string code, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<bool> IsEnabledAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }
}
