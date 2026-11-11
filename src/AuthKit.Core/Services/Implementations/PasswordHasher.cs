namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Configuration;
using AuthKit.Core.Services;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHashOptions _options;

    public PasswordHasher(PasswordHashOptions options)
    {
        _options = options;
    }

    public Task<string> HashAsync(string password, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Salt = salt;
        argon2.MemorySize = _options.Memory;
        argon2.Iterations = _options.Iterations;
        argon2.DegreeOfParallelism = _options.Parallelism;

        var hash = argon2.GetBytes(32);

        var saltB64 = Convert.ToBase64String(salt);
        var hashB64 = Convert.ToBase64String(hash);

        return Task.FromResult($"$argon2id$v=19$m={_options.Memory},t={_options.Iterations},p={_options.Parallelism}${saltB64}${hashB64}");
    }

    public Task<bool> VerifyAsync(string password, string hash, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(password))
            return Task.FromResult(false);

        var parts = hash.Split('$');
        // Format: $argon2id$v=19$m=X,t=Y,p=Z$salt$hash
        if (parts.Length != 6 || parts[1] != "argon2id" || parts[2] != "v=19")
            return Task.FromResult(false);

        var configParts = parts[3].Split(',');
        var saltB64 = parts[4];
        var expectedHashB64 = parts[5];

        var memory = int.Parse(configParts[0].Split('=')[1]);
        var iterations = int.Parse(configParts[1].Split('=')[1]);
        var parallelism = int.Parse(configParts[2].Split('=')[1]);

        var salt = Convert.FromBase64String(saltB64);
        var expectedHash = Convert.FromBase64String(expectedHashB64);

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Salt = salt;
        argon2.MemorySize = memory;
        argon2.Iterations = iterations;
        argon2.DegreeOfParallelism = parallelism;

        var computedHash = argon2.GetBytes(32);

        return Task.FromResult(CryptographicOperations.FixedTimeEquals(computedHash, expectedHash));
    }
}
