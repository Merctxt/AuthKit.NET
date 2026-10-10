using Microsoft.Extensions.Options;
using AuthKit.Core.Options;
using AuthKit.Core.Services;
using AuthKit.Interfaces.Repositories;

namespace AuthKit.Identity.Services;

public class RateLimitingService : IRateLimitingService
{
    private readonly IAuthRepository _repository;
    private readonly RateLimitingOptions _options;

    public RateLimitingService(
        IAuthRepository repository,
        IOptions<RateLimitingOptions> options)
    {
        _repository = repository;
        _options = options.Value;
    }

    public bool IsRateLimited(string identifier, string endpoint)
    {
        var attempts = GetFailedAttempts(identifier, endpoint);
        return attempts > 0;
    }

    public void RecordAttempt(string identifier, string endpoint, bool succeeded)
    {
        // Implementation stores in database via LoginAttempts table
        // Simplified for now
    }

    public int GetFailedAttempts(string identifier, string endpoint)
    {
        var attemptType = endpoint switch
        {
            "/auth/login" => "Login",
            "/auth/forgot-password" => "ForgotPassword",
            "/auth/register" => "Register",
            _ => "Login"
        };

        return _repository.GetFailedAttemptsCountAsync(identifier, attemptType).Result;
    }

    public void ClearAttempts(string identifier, string endpoint)
    {
        _repository.ClearLoginAttemptsAsync(identifier).Wait();
    }

    public int? GetRemainingAttempts(string identifier, string endpoint)
    {
        var maxAttempts = endpoint switch
        {
            "/auth/login" => _options.LoginMaxAttempts,
            "/auth/forgot-password" => _options.ForgotPasswordMaxAttempts,
            "/auth/register" => _options.RegisterMaxAttempts,
            _ => _options.LoginMaxAttempts
        };

        var current = GetFailedAttempts(identifier, endpoint);
        return Math.Max(0, maxAttempts - current);
    }
}
