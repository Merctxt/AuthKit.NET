namespace AuthKit.Tests.Configuration;

public class SessionTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public SessionTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_SessionZeroExpiration_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.Session = new SessionOptions { Expiration = TimeSpan.Zero });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("Session Expiration must be greater than zero."));
    }

    [Fact]
    public void ValidateAll_SessionMaxActiveOutOfRange_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.Session = new SessionOptions { MaxActivePerUser = 200 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("Session MaxActivePerUser must be between 1 and 100"));
    }
}
