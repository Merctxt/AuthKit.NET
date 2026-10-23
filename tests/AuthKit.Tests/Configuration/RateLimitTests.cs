namespace AuthKit.Tests.Configuration;

public class RateLimitTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public RateLimitTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_RateLimitDisabled_Passes()
    {
        var options = TestHelper.DefaultOptions(o => o.RateLimit = new RateLimitOptions { Enabled = false });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateAll_RateLimitZeroAttempts_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.RateLimit = new RateLimitOptions { Enabled = true, MaxAttempts = 0 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("RateLimit MaxAttempts must be at least 1."));
    }

    [Fact]
    public void ValidateAll_RateLimitZeroWindow_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.RateLimit = new RateLimitOptions { Enabled = true, Window = TimeSpan.Zero });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("RateLimit Window must be greater than zero."));
    }

    [Fact]
    public void ValidateAll_RateLimitWindowOver1Hour_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.RateLimit = new RateLimitOptions { Enabled = true, Window = TimeSpan.FromHours(2) });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("longer than 1 hour"));
    }

    [Fact]
    public void ValidateAll_RateLimitZeroLockoutDuration_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.RateLimit = new RateLimitOptions { Enabled = true, LockoutDuration = TimeSpan.Zero });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("LockoutDuration"));
    }
}
