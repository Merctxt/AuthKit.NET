namespace AuthKit.Tests.Configuration;

public class TwoFactorTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public TwoFactorTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_TwoFactorDisabled_Passes()
    {
        var options = TestHelper.DefaultOptions(o => o.TwoFactor = new TwoFactorOptions { Enabled = false });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateAll_TwoFactorDigitsOutOfRange_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.TwoFactor = new TwoFactorOptions { Enabled = true, Digits = 2 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("TwoFactor Digits must be between 4 and 10"));
    }

    [Fact]
    public void ValidateAll_TwoFactorPeriodZero_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.TwoFactor = new TwoFactorOptions { Enabled = true, Period = TimeSpan.Zero });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("TwoFactor Period must be between zero and 5 minutes"));
    }

    [Fact]
    public void ValidateAll_TwoFactorBackupCodesOutOfRange_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.TwoFactor = new TwoFactorOptions { Enabled = true, BackupCodesCount = 30 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("TwoFactor BackupCodesCount must be between 1 and 20"));
    }

    [Fact]
    public void ValidateAll_TwoFactorEmptyTOTPIssuer_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.TwoFactor = new TwoFactorOptions { Enabled = true, TOTPIssuer = "" });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("TOTPIssuer is empty"));
    }
}
