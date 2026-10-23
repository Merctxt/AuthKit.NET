namespace AuthKit.Tests.Configuration;

public class SmtpTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public SmtpTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_SmtpDisabled_Passes()
    {
        var options = TestHelper.DefaultOptions(o => o.Smtp = new SmtpOptions { Enabled = false });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateAll_SmtpEnabledMissingFields_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.Smtp = new SmtpOptions { Enabled = true, Host = "smtp.gmail.com" });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().ContainSingle().And.Contain(x => x.Contains("SMTP configuration is required"));
    }

    [Fact]
    public void ValidateAll_SmtpEnabledFullConfig_Passes()
    {
        var options = TestHelper.DefaultOptions(o => o.Smtp = new SmtpOptions
        {
            Enabled = true,
            Host = "smtp.gmail.com",
            Port = 587,
            UserName = "test@test.com",
            Password = "pass",
            FromEmail = "test@test.com",
        });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateAll_SmtpInvalidPort_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.Smtp = new SmtpOptions
        {
            Enabled = true,
            Host = "smtp.test.com",
            Port = 99999,
            UserName = "u",
            Password = "p",
            FromEmail = "test@test.com",
        });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().ContainSingle().And.Contain(x => x.Contains("SMTP configuration is required"));
    }

    [Fact]
    public void ValidateAll_SmtpPort25_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.Smtp = new SmtpOptions
        {
            Enabled = true,
            Host = "smtp.test.com",
            Port = 25,
            UserName = "u",
            Password = "p",
            FromEmail = "test@test.com",
        });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("Port 25"));
    }

    [Fact]
    public void ValidateAll_SmtpConnectionTimeoutLow_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.Smtp = new SmtpOptions
        {
            Enabled = true,
            Host = "smtp.test.com",
            Port = 587,
            UserName = "u",
            Password = "p",
            FromEmail = "test@test.com",
            ConnectionTimeout = 1000,
        });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("ConnectionTimeout"));
    }

    [Fact]
    public void ValidateAll_SmtpInvalidFromEmail_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.Smtp = new SmtpOptions
        {
            Enabled = true,
            Host = "smtp.test.com",
            Port = 587,
            UserName = "u",
            Password = "p",
            FromEmail = "invalid",
        });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("SMTP FromEmail must be a valid email address."));
    }
}
