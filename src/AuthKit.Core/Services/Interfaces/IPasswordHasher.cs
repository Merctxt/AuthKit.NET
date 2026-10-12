namespace AuthKit.Core.Services;

public interface IPasswordHasher
{
    Task<string> HashAsync(string password, CancellationToken ct = default);
    Task<bool> VerifyAsync(string password, string hash, CancellationToken ct = default);
}
