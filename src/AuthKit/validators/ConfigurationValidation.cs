namespace AuthKit.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public static class ConfigurationValidation
{
    private static readonly Type _loggerType = typeof(ConfigurationValidation);

    public static IApplicationBuilder ValidateConfiguration(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IOptions<AuthKitOptions>>().Value;
        var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(_loggerType);

        var errors = ValidateAll(options, logger);

        if (errors.Count > 0)
        {
            var message = string.Join("; ", errors);
            logger.LogError("AuthKit configuration validation failed: {Errors}", message);
            throw new InvalidOperationException(message);
        }

        return app;
    }

    public static List<string> ValidateAll(AuthKitOptions options, ILogger logger)
    {
        var errors = new List<string>();

        ValidateJwt(options.Jwt, logger);
        ValidatePasswordHash(options.PasswordHash, logger);
        ValidatePasswordPolicy(options.PasswordPolicy, errors, logger);
        ValidateRefreshToken(options.RefreshToken, errors);
        ValidateSmtp(options.Smtp, errors, logger);
        ValidateRateLimit(options.RateLimit, errors, logger);
        ValidateAccountLockout(options.AccountLockout, errors, logger);
        ValidateTwoFactor(options.TwoFactor, errors, logger);
        ValidateSession(options.Session, errors);
        ValidateUserValidation(options.UserValidation, errors, logger);
        ValidatePagination(options.Pagination, errors, logger);

        return errors;
    }

    private static void ValidateJwt(JwtOptions jwt, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(jwt.SecretKey))
        {
            throw new InvalidOperationException(
                "The JWT SecretKey is required. Set options.Jwt.SecretKey in your configuration.");
        }

        if (jwt.SecretKey!.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey must be at least 32 characters long. " +
                "Use a cryptographically secure random string (e.g., from `dotnet user-secrets` or `openssl rand -base64 32`).");
        }

        var supportedAlgorithms = new[] { "HS256", "HS384", "HS512" };
        if (!supportedAlgorithms.Contains(jwt.SigningAlgorithm, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Unsupported JWT signing algorithm '{jwt.SigningAlgorithm}'. " +
                $"Supported: {string.Join(", ", supportedAlgorithms)}.");
        }

        if (jwt.Expiration <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("JWT Expiration must be greater than zero.");
        }

        if (jwt.Expiration > TimeSpan.FromDays(1))
        {
            logger.LogWarning("JWT Expiration is set to {Expiration}. Tokens lasting more than 1 day are discouraged for security reasons.", jwt.Expiration);
        }

        if (jwt.Expiration < TimeSpan.FromMinutes(5))
        {
            logger.LogWarning("JWT Expiration is set to {Expiration}. Values less than 5 minutes may cause issues with clients in low-latency networks.", jwt.Expiration);
        }
    }

    private static void ValidatePasswordHash(PasswordHashOptions hash, ILogger logger)
    {
        if (hash.Iterations < 10)
        {
            logger.LogWarning(
                "PasswordHash Iterations is set to {Iterations}. Minimum recommended is 10.", hash.Iterations);
        }

        if (hash.Memory < 16384)
        {
            logger.LogWarning(
                "PasswordHash Memory is set to {Memory} bytes. Minimum recommended is 65536.", hash.Memory);
        }
    }

    private static void ValidatePasswordPolicy(PasswordPolicyOptions policy, List<string> errors, ILogger logger)
    {
        if (policy.MinLength < 6)
        {
            logger.LogWarning(
                "Password MinLength is set to {MinLength}. Minimum recommended is 8 characters.", policy.MinLength);
        }

        if (policy.MinLength > 128)
        {
            errors.Add("Password MinLength must not exceed 128 characters.");
        }

        if (policy.MaxRepeatedCharacters < 1 || policy.MaxRepeatedCharacters > 8)
        {
            errors.Add("Password MaxRepeatedCharacters must be between 1 and 8.");
        }
    }

    private static void ValidateRefreshToken(RefreshTokenOptions refreshToken, List<string> errors)
    {
        if (refreshToken.Expiration <= TimeSpan.Zero)
        {
            errors.Add("RefreshToken Expiration must be greater than zero.");
        }

        if (refreshToken.AbsoluteExpiration.HasValue && refreshToken.AbsoluteExpiration.Value <= TimeSpan.Zero)
        {
            errors.Add("RefreshToken AbsoluteExpiration must be greater than zero.");
        }

        if (refreshToken.AbsoluteExpiration.HasValue && refreshToken.AbsoluteExpiration.Value < refreshToken.Expiration)
        {
            errors.Add("RefreshToken AbsoluteExpiration must be greater than or equal to RefreshToken Expiration.");
        }
    }

    private static void ValidateSmtp(SmtpOptions smtp, List<string> errors, ILogger logger)
    {
        if (!smtp.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(smtp.Host)
            || smtp.Port < 1
            || smtp.Port > 65535
            || string.IsNullOrWhiteSpace(smtp.UserName)
            || string.IsNullOrWhiteSpace(smtp.Password)
            || string.IsNullOrWhiteSpace(smtp.FromEmail))
        {
            errors.Add(
                "SMTP configuration is required when email features are enabled. " +
                "Set options.Smtp.Host, options.Smtp.Port, options.Smtp.UserName, " +
                "options.Smtp.Password, and options.Smtp.FromEmail.");
            return;
        }

        if (!smtp.FromEmail.Contains("@"))
        {
            errors.Add("SMTP FromEmail must be a valid email address.");
        }

        if (smtp.Port == 25)
        {
            logger.LogWarning("SMTP Port 25 is typically used for server-to-server relay and may be blocked by cloud providers. Consider using port 587 with STARTTLS.");
        }

        if (smtp.ConnectionTimeout < 5000)
        {
            logger.LogWarning("SMTP ConnectionTimeout is set to {Timeout}ms. Values below 5000ms may cause unreliable connections.", smtp.ConnectionTimeout);
        }
    }

    private static void ValidateRateLimit(RateLimitOptions rateLimit, List<string> errors, ILogger logger)
    {
        if (!rateLimit.Enabled)
            return;

        if (rateLimit.MaxAttempts < 1)
        {
            errors.Add("RateLimit MaxAttempts must be at least 1.");
        }

        if (rateLimit.Window <= TimeSpan.Zero)
        {
            errors.Add("RateLimit Window must be greater than zero.");
        }

        if (rateLimit.Window > TimeSpan.FromHours(1))
        {
            logger.LogWarning("RateLimit Window is set to {Window}. Windows longer than 1 hour are unusual and may impact usability.", rateLimit.Window);
        }

        if (rateLimit.LockoutDuration <= TimeSpan.Zero)
        {
            logger.LogWarning("RateLimit LockoutDuration is set to zero. Rate limit will be enforced, but no account lockout will occur.");
        }
    }

    private static void ValidateAccountLockout(AccountLockoutOptions lockout, List<string> errors, ILogger logger)
    {
        if (!lockout.Enabled)
            return;

        if (lockout.MaxFailedAttempts < 1 || lockout.MaxFailedAttempts > 100)
        {
            errors.Add("AccountLockout MaxFailedAttempts must be between 1 and 100.");
        }

        if (lockout.DefaultLockoutTime <= TimeSpan.Zero)
        {
            errors.Add("AccountLockout DefaultLockoutTime must be greater than zero.");
        }

        if (lockout.DefaultLockoutTime > TimeSpan.FromDays(1))
        {
            logger.LogWarning(
                "AccountLockout DefaultLockoutTime is set to {Time}. Lockouts longer than 1 day may frustrate users.",
                lockout.DefaultLockoutTime);
        }
    }

    private static void ValidateTwoFactor(TwoFactorOptions twoFactor, List<string> errors, ILogger logger)
    {
        if (!twoFactor.Enabled)
            return;

        if (twoFactor.Digits < 4 || twoFactor.Digits > 10)
        {
            errors.Add("TwoFactor Digits must be between 4 and 10.");
        }

        if (twoFactor.Period <= TimeSpan.Zero || twoFactor.Period > TimeSpan.FromMinutes(5))
        {
            errors.Add("TwoFactor Period must be between zero and 5 minutes.");
        }

        if (twoFactor.BackupCodesCount < 1 || twoFactor.BackupCodesCount > 20)
        {
            errors.Add("TwoFactor BackupCodesCount must be between 1 and 20.");
        }

        if (string.IsNullOrWhiteSpace(twoFactor.TOTPIssuer))
        {
            logger.LogWarning("TwoFactor TOTPIssuer is empty. The TOTP issuer name will not be displayed in authenticator apps.");
        }
    }

    private static void ValidateSession(SessionOptions session, List<string> errors)
    {
        if (session.Expiration <= TimeSpan.Zero)
        {
            errors.Add("Session Expiration must be greater than zero.");
        }

        if (session.MaxActivePerUser < 1 || session.MaxActivePerUser > 100)
        {
            errors.Add("Session MaxActivePerUser must be between 1 and 100.");
        }
    }

    private static void ValidateUserValidation(UserValidationOptions validation, List<string> errors, ILogger logger)
    {
        if (validation.UsernameMinLength < 1 || validation.UsernameMinLength > 100)
        {
            errors.Add("UserValidation UsernameMinLength must be between 1 and 100.");
        }

        if (validation.UsernameMaxLength < validation.UsernameMinLength)
        {
            errors.Add("UserValidation UsernameMaxLength must be greater than or equal to UsernameMinLength.");
        }

        if (validation.UsernameMaxLength > 256)
        {
            errors.Add("UserValidation UsernameMaxLength must not exceed 256 characters.");
        }

        if (!string.IsNullOrWhiteSpace(validation.UsernamePattern))
        {
            try
            {
                System.Text.RegularExpressions.Regex.IsMatch("test", validation.UsernamePattern);
            }
            catch (Exception ex) when (ex is FormatException || ex is System.Text.RegularExpressions.RegexParseException)
            {
                errors.Add($"UserValidation UsernamePattern is not a valid regular expression: {ex.Message}");
            }
        }
    }

    private static void ValidatePagination(PaginationOptions pagination, List<string> errors, ILogger logger)
    {
        if (pagination.DefaultPageSize < 1 || pagination.DefaultPageSize > 1000)
        {
            errors.Add("Pagination DefaultPageSize must be between 1 and 1000.");
        }

        if (pagination.MaxPageSize < pagination.DefaultPageSize)
        {
            errors.Add("Pagination MaxPageSize must be greater than or equal to DefaultPageSize.");
        }

        if (pagination.MaxPageSize > 1000)
        {
            logger.LogWarning(
                "Pagination MaxPageSize is set to {MaxPageSize}. Values above 1000 may impact performance.",
                pagination.MaxPageSize);
        }
    }
}
