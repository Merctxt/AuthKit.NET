namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Services;

public class PasswordHasher : IPasswordHasher
{
    public Task<string> HashAsync(string password, CancellationToken ct = default)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<bool> VerifyAsync(string password, string hash, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }
}
