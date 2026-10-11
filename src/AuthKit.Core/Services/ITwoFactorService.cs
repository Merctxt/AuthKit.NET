namespace AuthKit.Core.Services;

using AuthKit.Core.Services.DTOs;

public interface ITwoFactorService
{
    Task<TwoFactorResponse> EnableAsync(Guid userId, EnableTwoFactorRequest request, CancellationToken ct = default);
    Task<IResult> DisableAsync(Guid userId, DisableTwoFactorRequest request, CancellationToken ct = default);
    Task<IResult> VerifyCodeAsync(Guid userId, VerifyTwoFactorRequest request, CancellationToken ct = default);
    Task<TwoFactorResponse> GenerateBackupCodesAsync(Guid userId, CancellationToken ct = default);
    Task<IResult> VerifyBackupCodeAsync(Guid userId, string code, CancellationToken ct = default);
    Task<bool> IsEnabledAsync(Guid userId, CancellationToken ct = default);
}
