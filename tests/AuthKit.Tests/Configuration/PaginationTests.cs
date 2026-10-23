namespace AuthKit.Tests.Configuration;

public class PaginationTests
{
    private readonly ILogger _logger;
    private readonly List<string> _warnings;

    public PaginationTests()
    {
        (this._logger, this._warnings) = TestHelper.CreateLogger();
    }

    [Fact]
    public void ValidateAll_PaginationDefaultOver1000_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.Pagination = new PaginationOptions { DefaultPageSize = 2000 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("Pagination DefaultPageSize must be between 1 and 1000"));
    }

    [Fact]
    public void ValidateAll_PaginationMaxUnderDefault_Fails()
    {
        var options = TestHelper.DefaultOptions(o => o.Pagination = new PaginationOptions { DefaultPageSize = 50, MaxPageSize = 10 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().Contain(x => x.Contains("Pagination MaxPageSize must be greater than or equal to DefaultPageSize"));
    }

    [Fact]
    public void ValidateAll_PaginationMaxOver1000_Warns()
    {
        var options = TestHelper.DefaultOptions(o => o.Pagination = new PaginationOptions { DefaultPageSize = 20, MaxPageSize = 2000 });

        var errors = ConfigurationValidation.ValidateAll(options, _logger);

        errors.Should().BeEmpty();
        _warnings.Should().Contain(w => w.Contains("MaxPageSize"));
    }
}
