namespace AuthKit.Tests.Configuration;

public class UserValidationTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public UserValidationTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_UserValidationMinLengthOutOfRange_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.UserValidation = new UserValidationOptions { UsernameMinLength = 0 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("UserValidation UsernameMinLength must be between 1 and 100"));
    }

    [Fact]
    public void ValidateAll_UserValidationMaxUnderMin_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.UserValidation = new UserValidationOptions { UsernameMinLength = 10, UsernameMaxLength = 5 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("UserValidation UsernameMaxLength must be greater than or equal to UsernameMinLength"));
    }

    [Fact]
    public void ValidateAll_UserValidationMaxOver256_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.UserValidation = new UserValidationOptions { UsernameMaxLength = 300 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("UserValidation UsernameMaxLength must not exceed 256 characters"));
    }

    [Fact]
    public void ValidateAll_UserValidationInvalidRegex_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.UserValidation = new UserValidationOptions { UsernamePattern = "[invalid(" });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().ContainSingle().And.Contain(x => x.Contains("regular expression"));
    }

    [Fact]
    public void ValidateAll_UserValidationValidPattern_Passes()
    {
        var options = TestHelper.DefaultOptions(o => o.UserValidation = new UserValidationOptions { UsernamePattern = @"^[a-zA-Z0-9_]+$" });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
    }
}
