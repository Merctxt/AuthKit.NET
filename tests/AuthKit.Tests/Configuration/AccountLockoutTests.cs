namespace AuthKit.Tests.Configuration;

public class AccountLockoutTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public AccountLockoutTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_AccountLockoutDisabled_Passes()
    {
        var options = TestHelper.DefaultOptions(o => o.AccountLockout = new AccountLockoutOptions { Enabled = false });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateAll_AccountLockoutMaxAttemptsOutOfRange_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.AccountLockout = new AccountLockoutOptions { Enabled = true, MaxFailedAttempts = 0 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("AccountLockout MaxFailedAttempts must be between 1 and 100"));
    }

    [Fact]
    public void ValidateAll_AccountLockoutZeroTime_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.AccountLockout = new AccountLockoutOptions { Enabled = true, DefaultLockoutTime = TimeSpan.Zero });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("AccountLockout DefaultLockoutTime must be greater than zero."));
    }

    [Fact]
    public void ValidateAll_AccountLockoutOver1Day_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.AccountLockout = new AccountLockoutOptions { Enabled = true, DefaultLockoutTime = TimeSpan.FromDays(2) });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("longer than 1 day"));
    }
}
