using AuthKit.Core.Options;
using AuthKit.Core.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace AuthKit.AuthKit.Services;

public class PasswordHashService : IPasswordHasher
{
    private readonly PasswordHashingOptions _options;

    public PasswordHashService(PasswordHashingOptions options)
    {
        _options = options;
    }

    public string HashPassword(string password)
    {
        return _options.Algorithm switch
        {
            HashingAlgorithm.Pbkdf2 => HashWithPbkdf2(password, "sha256"),
            HashingAlgorithm.BCrypt => HashWithBCrypt(password),
            _ => throw new NotSupportedException($"Algorithm {_options.Algorithm} is not supported.")
        };
    }

    public bool VerifyPassword(string password, string hash)
    {
        return _options.Algorithm switch
        {
            HashingAlgorithm.Pbkdf2 => VerifyWithPbkdf2(password, hash),
            HashingAlgorithm.BCrypt => BCrypt.Net.BCrypt.Verify(password, hash),
            _ => throw new NotSupportedException($"Algorithm {_options.Algorithm} is not supported.")
        };
    }

    private string HashWithPbkdf2(string password, string hashAlgorithm)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var prf = hashAlgorithm.ToLower() switch
        {
            "sha256" => KeyDerivationPrf.HMACSHA256,
            "sha512" => KeyDerivationPrf.HMACSHA512,
            _ => KeyDerivationPrf.HMACSHA256
        };

        var hashBytes = KeyDerivation.Pbkdf2(
            password,
            salt,
            prf,
            _options.Argon2Iterations,
            32);

        var result = new byte[20 + salt.Length + hashBytes.Length];
        result[0] = 0x01;
        result[1] = 0x00;
        result[2] = 0x00;
        result[3] = 0x00;
        result[4] = (byte)prf;
        Buffer.BlockCopy(BitConverter.GetBytes(_options.Argon2Iterations), 0, result, 5, 4);
        Buffer.BlockCopy(salt, 0, result, 9, salt.Length);
        Buffer.BlockCopy(hashBytes, 0, result, 25, hashBytes.Length);

        return Convert.ToBase64String(result);
    }

    private bool VerifyWithPbkdf2(string password, string hashedPassword)
    {
        var hashBytes = Convert.FromBase64String(hashedPassword);

        if (hashBytes.Length < 57)
        {
            return false;
        }

        var version = hashBytes[0];
        var prf = (KeyDerivationPrf)hashBytes[4];
        var iterations = BitConverter.ToInt32(hashBytes, 5);
        var salt = new byte[16];
        Buffer.BlockCopy(hashBytes, 9, salt, 0, 16);
        var storedHash = new byte[32];
        Buffer.BlockCopy(hashBytes, 25, storedHash, 0, 32);

        var computedHash = KeyDerivation.Pbkdf2(password, salt, prf, iterations, 32);
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }

    private string HashWithBCrypt(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, _options.BCryptStrength);
    }
}
