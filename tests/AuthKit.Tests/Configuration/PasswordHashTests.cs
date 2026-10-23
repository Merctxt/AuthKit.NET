namespace AuthKit.Tests.Configuration;

public class PasswordHashTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public PasswordHashTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_PasswordHashLowIterations_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.PasswordHash = new PasswordHashOptions { Iterations = 3, Memory = 65536 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("Iterations") && w.Contains("3"));
    }

    [Fact]
    public void ValidateAll_PasswordHashLowMemory_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.PasswordHash = new PasswordHashOptions { Iterations = 10, Memory = 8192 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("Memory") && w.Contains("8192"));
    }
}
