namespace AuthKit.Tests.Services;

using AuthKit.Core.Configuration;
using AuthKit.Core.Services.Implementations;
using Microsoft.IdentityModel.Tokens;

public class PasswordHasherTests
{
    private readonly PasswordHashOptions _defaultOptions = new()
    {
        Memory = 65536,
        Iterations = 3,
        Parallelism = 1,
    };

    [Fact]
    public async Task HashAsync_ReturnsProperFormat()
    {
        var hasher = new PasswordHasher(_defaultOptions);
        var hash = await hasher.HashAsync("Password123");

        hash.Should().StartWith("$argon2id$v=19$");
    }

    [Fact]
    public async Task HashAsync_DifferentCallsProducesDifferentHashes()
    {
        var hasher = new PasswordHasher(_defaultOptions);
        var hash1 = await hasher.HashAsync("Password123");
        var hash2 = await hasher.HashAsync("Password123");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public async Task VerifyAsync_ValidPassword_ReturnsTrue()
    {
        var hasher = new PasswordHasher(_defaultOptions);
        var hash = await hasher.HashAsync("Password123");
        var result = await hasher.VerifyAsync("Password123", hash);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyAsync_IncorrectPassword_ReturnsFalse()
    {
        var hasher = new PasswordHasher(_defaultOptions);
        var hash = await hasher.HashAsync("Password123");
        var result = await hasher.VerifyAsync("WrongPassword", hash);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyAsync_CorruptedHash_ReturnsFalse()
    {
        var hasher = new PasswordHasher(_defaultOptions);
        var result = await hasher.VerifyAsync("Password123", "$argon2id$v=19$m=65536,t=3,p=1$YWJjZGVm$aW52YWxpZA==");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyAsync_EmptyInput_ReturnsFalse()
    {
        var hasher = new PasswordHasher(_defaultOptions);
        var hash = await hasher.HashAsync("Password123");
        var result = await hasher.VerifyAsync("", hash);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task HashAndVerify_VariousPasswords()
    {
        var hasher = new PasswordHasher(_defaultOptions);

        var passwords = new[] { "Short1!", "LongPasswordWithNumbersAndSymbols@2024", "Aa1!Bb2@Cc3#" };

        foreach (var password in passwords)
        {
            var hash = await hasher.HashAsync(password);
            var result = await hasher.VerifyAsync(password, hash);

            result.Should().BeTrue($"Password '{password}' should hash and verify.");
        }
    }

    [Fact]
    public async Task HashAsync_RespectsConfigurationOptions()
    {
        var strictOptions = new PasswordHashOptions { Memory = 32768, Iterations = 2, Parallelism = 1 };
        var hasher = new PasswordHasher(strictOptions);
        var hash = await hasher.HashAsync("Password123");

        hash.Should().Contain("m=32768");
        hash.Should().Contain("t=2");
        hash.Should().Contain("p=1");
    }
}
