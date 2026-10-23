namespace AuthKit.Tests.Configuration;

public class JwtTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public JwtTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_DefaultOptions_Passes()
    {
        var options = TestHelper.DefaultOptions();

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateAll_JwtNoSecretKey_Throws()
    {
        var options = new AuthKitOptions
        {
            Jwt = new JwtOptions(),
        };

        var act = () => ConfigurationValidation.ValidateAll(options, _logger);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The JWT SecretKey is required.*");
    }

    [Fact]
    public void ValidateAll_JwtShortSecretKey_Throws()
    {
        var options = TestHelper.DefaultOptions(o => o.Jwt.SecretKey = "short");

        var act = () => ConfigurationValidation.ValidateAll(options, _logger);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT SecretKey must be at least 32 characters long.*");
    }

    [Fact]
    public void ValidateAll_JwtUnsupportedAlgorithm_Throws()
    {
        var options = TestHelper.DefaultOptions(o => o.Jwt.SigningAlgorithm = "RSA123");

        var act = () => ConfigurationValidation.ValidateAll(options, _logger);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Unsupported JWT signing algorithm 'RSA123'.*");
    }

    [Fact]
    public void ValidateAll_JwtZeroExpiration_Throws()
    {
        var options = TestHelper.DefaultOptions(o => o.Jwt.Expiration = TimeSpan.Zero);

        var act = () => ConfigurationValidation.ValidateAll(options, _logger);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT Expiration must be greater than zero.");
    }

    [Fact]
    public void ValidateAll_JwtExpirationOver1Day_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.Jwt.Expiration = TimeSpan.FromDays(2));

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("more than 1 day"));
    }

    [Fact]
    public void ValidateAll_JwtExpirationUnder5Minutes_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.Jwt.Expiration = TimeSpan.FromMinutes(2));

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("less than 5 minutes"));
    }

    [Fact]
    public void ValidateAll_JwtValidAlgorithms_DoesNotThrow()
    {
        var options = TestHelper.DefaultOptions();

        var errors1 = ConfigurationValidation.ValidateAll(options, _logger);
        errors1.Should().BeEmpty();

        options.Jwt.SigningAlgorithm = "hs384";
        var errors2 = ConfigurationValidation.ValidateAll(options, _logger);
        errors2.Should().BeEmpty();

        options.Jwt.SigningAlgorithm = "hs512";
        var errors3 = ConfigurationValidation.ValidateAll(options, _logger);
        errors3.Should().BeEmpty();
    }
}
