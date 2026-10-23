namespace AuthKit.Tests.Configuration;

public class MultipleErrorsTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public MultipleErrorsTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_MultipleErrors_AllCollected()
    {
        var options = new AuthKitOptions
        {
            Jwt = new JwtOptions { SecretKey = "T3h!K8m#pL2qR$vN5wX7yA0bC4dF9gH1jK3mP6rS8tU0xZ2cE5fI7lO1qT4vW8yB" },
            RefreshToken = new RefreshTokenOptions { Expiration = TimeSpan.Zero },
            Session = new SessionOptions { Expiration = TimeSpan.Zero, MaxActivePerUser = 0 },
            UserValidation = new UserValidationOptions { UsernameMinLength = 0, UsernamePattern = "[invalid(" },
            Pagination = new PaginationOptions { DefaultPageSize = 5000, MaxPageSize = 10 },
        };

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().HaveCountGreaterThanOrEqualTo(6);
    }
}
