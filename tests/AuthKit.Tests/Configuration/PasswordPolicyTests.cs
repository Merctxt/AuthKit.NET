namespace AuthKit.Tests.Configuration;

public class PasswordPolicyTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public PasswordPolicyTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_PasswordPolicyValid_Passes()
    {
        var options = TestHelper.DefaultOptions(o => o.PasswordPolicy = new PasswordPolicyOptions
        {
            MinLength = 8,
            RequireUppercase = true,
            RequireLowercase = true,
            RequireDigit = true,
            RequireNonAlphanumeric = true,
            MaxRepeatedCharacters = 3,
        });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateAll_PasswordPolicyMinLengthUnder6_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.PasswordPolicy = new PasswordPolicyOptions { MinLength = 4 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("Minimum recommended is 8"));
    }

    [Fact]
    public void ValidateAll_PasswordPolicyMinLengthOver128_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.PasswordPolicy = new PasswordPolicyOptions { MinLength = 200 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().ContainSingle().And.Contain(x => x.Contains("Password MinLength must not exceed 128"));
    }

    [Fact]
    public void ValidateAll_PasswordPolicyMaxRepeatedOutOfRange_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.PasswordPolicy = new PasswordPolicyOptions { MaxRepeatedCharacters = 10 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("Password MaxRepeatedCharacters must be between 1 and 8."));
    }
}
