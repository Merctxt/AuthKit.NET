namespace AuthKit.Tests.Configuration;

public class RefreshTokenTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public RefreshTokenTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_RefreshTokenValid_Passes()
    {
        var options = TestHelper.DefaultOptions(o => o.RefreshToken = new RefreshTokenOptions
        {
            Expiration = TimeSpan.FromDays(7),
            AbsoluteExpiration = TimeSpan.FromDays(30),
        });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateAll_RefreshTokenZeroExpiration_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.RefreshToken = new RefreshTokenOptions { Expiration = TimeSpan.Zero });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("RefreshToken Expiration must be greater than zero."));
    }

    [Fact]
    public void ValidateAll_RefreshTokenAbsoluteUnderExpiration_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.RefreshToken = new RefreshTokenOptions
        {
            Expiration = TimeSpan.FromDays(7),
            AbsoluteExpiration = TimeSpan.FromDays(3),
        });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("RefreshToken AbsoluteExpiration must be greater than or equal to RefreshToken Expiration."));
    }

    [Fact]
    public void ValidateAll_RefreshTokenZeroAbsolute_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.RefreshToken = new RefreshTokenOptions
        {
            Expiration = TimeSpan.FromDays(7),
            AbsoluteExpiration = TimeSpan.Zero,
        });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("RefreshToken AbsoluteExpiration must be greater than zero."));
    }
}
