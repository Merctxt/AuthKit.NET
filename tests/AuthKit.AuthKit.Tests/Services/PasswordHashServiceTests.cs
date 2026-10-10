using AuthKit.Core.Options;
using AuthKit.AuthKit.Services;

namespace AuthKit.AuthKit.Tests.Services;

public class PasswordHashServiceTests
{
    private readonly PasswordHashService _pbkdf2Service;
    private readonly PasswordHashService _bcryptService;

    public PasswordHashServiceTests()
    {
        _pbkdf2Service = new PasswordHashService(new PasswordHashingOptions
        {
            Algorithm = HashingAlgorithm.Pbkdf2
        });

        _bcryptService = new PasswordHashService(new PasswordHashingOptions
        {
            Algorithm = HashingAlgorithm.BCrypt,
            BCryptStrength = 10
        });
    }

    [Fact]
    public void HashPassword_Pbkdf2_ReturnsNonEmptyString()
    {
        // Arrange & Act
        var hash = _pbkdf2Service.HashPassword("TestPassword123!");

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashPassword_Pbkdf2_DifferentHashesForEachCall()
    {
        // Arrange & Act
        var hash1 = _pbkdf2Service.HashPassword("TestPassword123!");
        var hash2 = _pbkdf2Service.HashPassword("TestPassword123!");

        // Assert - should be different due to random salt
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPassword_Pbkdf2_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hash = _pbkdf2Service.HashPassword(password);

        // Act
        var result = _pbkdf2Service.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_Pbkdf2_WithWrongPassword_ReturnsFalse()
    {
        // Arrange
        var correctPassword = "CorrectPassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = _pbkdf2Service.HashPassword(correctPassword);

        // Act
        var result = _pbkdf2Service.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HashPassword_BCrypt_ReturnsNonEmptyString()
    {
        // Arrange & Act
        var hash = _bcryptService.HashPassword("TestPassword123!");

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.StartsWith("$2", hash);
    }

    [Fact]
    public void VerifyPassword_BCrypt_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hash = _bcryptService.HashPassword(password);

        // Act
        var result = _bcryptService.VerifyPassword(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_BCrypt_WithWrongPassword_ReturnsFalse()
    {
        // Arrange
        var correctPassword = "CorrectPassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = _bcryptService.HashPassword(correctPassword);

        // Act
        var result = _bcryptService.VerifyPassword(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HashPassword_WithNullPassword_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _pbkdf2Service.HashPassword(null!));
    }

    [Fact]
    public void HashPassword_SupportsUnrecognizedAlgorithm_ThrowsException()
    {
        // Arrange
        var options = new PasswordHashingOptions
        {
            Algorithm = (HashingAlgorithm)999
        };
        var service = new PasswordHashService(options);

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => service.HashPassword("test"));
    }
}
