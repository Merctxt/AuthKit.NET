using AuthKit.Core.Options;

namespace AuthKit.Core.Tests.Options;

public class DatabaseOptionsTests
{
    [Fact]
    public void DatabaseOptions_Defaults_AreCorrect()
    {
        // Arrange & Act
        var options = new DatabaseOptions();

        // Assert
        Assert.Equal(string.Empty, options.ConnectionString);
        Assert.True(options.AutoMigrate);
        Assert.True(options.AutoSeed);
    }
}

public class JwtOptionsTests
{
    [Fact]
    public void JwtOptions_Defaults_AreCorrect()
    {
        // Arrange & Act
        var options = new JwtOptions();

        // Assert
        Assert.Equal(string.Empty, options.Secret);
        Assert.Equal(TimeSpan.FromMinutes(15), options.AccessExpiration);
        Assert.Equal(TimeSpan.FromDays(7), options.RefreshExpiration);
        Assert.Equal("AuthKit.NET", options.Issuer);
        Assert.Equal(1, options.ClockSkewMinutes);
    }
}

public class RefreshTokenOptionsTests
{
    [Fact]
    public void RefreshTokenOptions_Defaults_AreCorrect()
    {
        // Arrange & Act
        var options = new RefreshTokenOptions();

        // Assert
        Assert.True(options.EnableRotation);
        Assert.Equal(1, options.MaxReuseCount);
        Assert.Equal(TimeSpan.FromDays(30), options.AbsoluteExpiry);
        Assert.Equal(TimeSpan.FromDays(7), options.SlidingExpiry);
    }
}

public class PasswordHashingOptionsTests
{
    [Fact]
    public void PasswordHashingOptions_DefaultAlgorithm_IsPbkdf2()
    {
        // Arrange & Act
        var options = new PasswordHashingOptions();

        // Assert
        Assert.Equal(HashingAlgorithm.Pbkdf2, options.Algorithm);
        Assert.Equal(12, options.BCryptStrength);
    }
}

public class RateLimitingOptionsTests
{
    [Fact]
    public void RateLimitingOptions_Defaults_AreCorrect()
    {
        // Arrange & Act
        var options = new RateLimitingOptions();

        // Assert
        Assert.True(options.Enabled);
        Assert.Equal(5, options.LoginMaxAttempts);
        Assert.Equal(TimeSpan.FromMinutes(5), options.LoginWindow);
        Assert.Equal(10, options.LoginByAccountMaxAttempts);
        Assert.Equal(TimeSpan.FromMinutes(10), options.LoginByAccountWindow);
    }
}

public class TwoFactorOptionsTests
{
    [Fact]
    public void TwoFactorOptions_Defaults_AreCorrect()
    {
        // Arrange & Act
        var options = new TwoFactorOptions();

        // Assert
        Assert.False(options.Enabled);
        Assert.Equal(6, options.TotpDigits);
        Assert.Equal(TimeSpan.FromSeconds(30), options.TotpPeriod);
        Assert.Equal(10, options.BackupCodesCount);
    }
}

public class MultiTenancyOptionsTests
{
    [Fact]
    public void MultiTenancyOptions_Defaults_AreCorrect()
    {
        // Arrange & Act
        var options = new MultiTenancyOptions();

        // Assert
        Assert.False(options.Enabled);
        Assert.True(options.RequireTenant);
    }
}

public class ScalarOptionsTests
{
    [Fact]
    public void ScalarOptions_Defaults_AreCorrect()
    {
        // Arrange & Act
        var options = new ScalarOptions();

        // Assert
        Assert.True(options.Enabled);
        Assert.Equal("/auth/docs", options.RoutePrefix);
        Assert.Equal("dark", options.Theme);
    }
}

public class MigrationOptionsTests
{
    [Fact]
    public void MigrationOptions_Defaults_AreCorrect()
    {
        // Arrange & Act
        var options = new MigrationOptions();

        // Assert
        Assert.True(options.ApplyOnStartup);
        Assert.True(options.EnforceForeignKeys);
        Assert.Equal(3, options.RetryCount);
        Assert.Equal(500, options.RetryDelayMs);
    }
}

public class SeedOptionsTests
{
    [Fact]
    public void SeedOptions_Defaults_AreCorrect()
    {
        // Arrange & Act
        var options = new SeedOptions();

        // Assert
        Assert.True(options.EnableDefaultSeed);
        Assert.True(options.CreateDefaultAdmin);
        Assert.Equal("admin@example.com", options.DefaultAdminEmail);
        Assert.Equal("Ch@ngeM3!Admin", options.DefaultAdminPassword);
        Assert.Equal("Admin", options.DefaultAdminRole);
    }
}
