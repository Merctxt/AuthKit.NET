using Microsoft.Extensions.Options;
using AuthKit.Core.Options;
using AuthKit.Core.Services;
using AuthKit.Interfaces.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace AuthKit.Identity.Services;

public class MfaService : IMfaService
{
    private readonly IAuthRepository _repository;
    private readonly TwoFactorOptions _options;

    public MfaService(
        IAuthRepository repository,
        IOptions<TwoFactorOptions> options)
    {
        _repository = repository;
        _options = options.Value;
    }

    public async Task<string> GenerateSecretKeyAsync(Guid userId)
    {
        var secret = new byte[_options.TotpEntropy];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(secret);

        var secretKey = Convert.ToBase64String(secret);

        await _repository.CreateOrUpdateTwoFactorSecretAsync(new Core.Models.TwoFactorSecret
        {
            UserId = userId,
            ProviderType = "TOTP",
            Secret = secretKey,
            IsEnabled = false,
            BackupCodesHashed = string.Empty
        });

        return secretKey;
    }

    public async Task<bool> VerifySecretKeyAsync(Guid userId, string secretKey, string code)
    {
        var secret = await _repository.GetTwoFactorSecretAsync(userId);
        if (secret == null || string.IsNullOrEmpty(secret.Secret))
            return false;

        // Simplified TOTP verification
        return VerifyTotpCode(secret.Secret, code, _options.TotpPeriod);
    }

    public async Task EnableTwoFactorAsync(Guid userId, string secret, IEnumerable<string> backupCodes)
    {
        var hashedBackupCodes = backupCodes.Select(code =>
            Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(code)))).ToList();

        await _repository.CreateOrUpdateTwoFactorSecretAsync(new Core.Models.TwoFactorSecret
        {
            UserId = userId,
            ProviderType = "TOTP",
            Secret = secret,
            IsEnabled = true,
            BackupCodesHashed = string.Join("|", hashedBackupCodes)
        });
    }

    public async Task DisableTwoFactorAsync(Guid userId)
    {
        await _repository.DeleteTwoFactorSecretAsync(userId);
    }

    public async Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code)
    {
        var secret = await _repository.GetTwoFactorSecretAsync(userId);
        if (secret == null || !secret.IsEnabled)
            return false;

        return VerifyTotpCode(secret.Secret, code, _options.TotpPeriod);
    }

    public async Task<bool> VerifyBackupCodeAsync(Guid userId, string code)
    {
        var secret = await _repository.GetTwoFactorSecretAsync(userId);
        if (secret == null) return false;

        var codeHash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(code)));

        var storedCodes = secret.BackupCodesHashed.Split('|');
        return storedCodes.Contains(codeHash);
    }

    public async Task<IEnumerable<string>> GenerateBackupCodesAsync(Guid userId)
    {
        var codes = new List<string>();
        for (int i = 0; i < _options.BackupCodesCount; i++)
        {
            var bytes = new byte[_options.BackupCodesLength];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            codes.Add(Convert.ToBase64String(bytes)[.._options.BackupCodesLength]);
        }
        return codes;
    }

    private bool VerifyTotpCode(string secret, string providedCode, TimeSpan period)
    {
        // Simplified TOTP verification
        // In production, use a proper TOTP library like Otp.NET
        try
        {
            var key = Convert.FromBase64String(secret);
            var timeStep = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds / period.TotalSeconds;

            // Generate code from time step and key
            using var hmac = new System.Security.Cryptography.HMACSHA1(key);
            var timeStepBytes = BitConverter.GetBytes(timeStep);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timeStepBytes);

            var hash = hmac.ComputeHash(timeStepBytes);
            var offset = hash[hash.Length - 1] & 0x0F;
            var binary = (hash[offset] & 0x7F) << 24
                       | (hash[offset + 1] & 0xFF) << 16
                       | (hash[offset + 2] & 0xFF) << 8
                       | (hash[offset + 3] & 0xFF);

            var code = binary % (int)Math.Pow(10, _options.TotpDigits);
            var expectedCode = code.ToString().PadLeft(_options.TotpDigits, '0');

            return expectedCode == providedCode;
        }
        catch
        {
            return false;
        }
    }
}
