using Microsoft.Extensions.Options;
using AuthKit.Core.Models;
using AuthKit.Core.Options;
using AuthKit.Core.Services;
using AuthKit.Interfaces.Repositories;

namespace AuthKit.Identity.Services;

public class LoginService : ILoginService
{
    private readonly IAuthRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMfaService _mfaService;
    private readonly RateLimitingOptions _rateLimitOptions;
    private readonly TwoFactorOptions _twoFactorOptions;
    private readonly ISessionService _sessionService;

    public LoginService(
        IAuthRepository repository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IMfaService mfaService,
        IOptions<RateLimitingOptions> rateLimitOptions,
        IOptions<TwoFactorOptions> twoFactorOptions,
        ISessionService sessionService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _mfaService = mfaService;
        _rateLimitOptions = rateLimitOptions.Value;
        _twoFactorOptions = twoFactorOptions.Value;
        _sessionService = sessionService;
    }

    public async Task<LoginResult> LoginAsync(string email, string password, string? ipAddress = null)
    {
        // Find tenant by default (simplified)
        var tenant = await _repository.GetDefaultTenantAsync();
        if (tenant == null)
            return new LoginResult(false, null, null, null, "No tenant configured.");

        var user = await _repository.GetUserByEmailAsync(email, tenant.Id);
        if (user == null)
            return new LoginResult(false, null, null, null, "Invalid credentials.");

        if (!user.EmailConfirmed)
            return new LoginResult(false, null, null, null, "Email not confirmed.");

        if (!user.TenantId.Equals(tenant.Id))
            return new LoginResult(false, null, null, null, "User not found in this tenant.");

        var passwordValid = _passwordHasher.VerifyPassword(password, user.PasswordHash);
        if (!passwordValid)
        {
            await _repository.CreateLoginAttemptAsync(new LoginAttempt
            {
                Identifier = email,
                AttemptType = "Login",
                IpAddress = ipAddress,
                Succeeded = false
            });

            return new LoginResult(false, null, null, null, "Invalid credentials.");
        }

        // Check 2FA
        if (_twoFactorOptions.Enabled)
        {
            var twoFactor = await _repository.GetTwoFactorSecretAsync(user.Id);
            if (twoFactor?.IsEnabled == true)
            {
                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _repository.UpdateUserAsync(user);

                return new LoginResult(
                    true,
                    null,
                    null,
                    "code",
                    "Two factor authentication required.");
            }
        }

        // Generate tokens
        var roles = await _repository.GetUserRolesAsync(user.Id);
        var roleNames = roles.Select(r => r.Name);
        var claims = await _repository.GetUserClaimsAsync(user.Id);
        var claimValues = claims.Select(c => $"{c.ClaimType}:{c.ClaimValue}");

        var (accessToken, refreshToken) = _tokenService.GenerateTokenPair(
            user.Id, user.Email, roleNames, [], claimValues);

        // Create session
        await _sessionService.CreateSessionAsync(user.Id, ipAddress ?? "Unknown");

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _repository.UpdateUserAsync(user);

        // Record successful login
        await _repository.CreateLoginAttemptAsync(new LoginAttempt
        {
            Identifier = email,
            AttemptType = "Login",
            IpAddress = ipAddress,
            Succeeded = true
        });

        return new LoginResult(
            true,
            accessToken,
            refreshToken,
            null,
            "Login successful.",
            roleNames.ToList());
    }

    public async Task<LoginResult> LoginWithTwoFactorAsync(string email, string code)
    {
        var tenant = await _repository.GetDefaultTenantAsync();
        if (tenant == null)
            return new LoginResult(false, null, null, null, "No tenant configured.");

        var user = await _repository.GetUserByEmailAsync(email, tenant.Id);
        if (user == null)
            return new LoginResult(false, null, null, null, "Invalid credentials.");

        var verified = await _mfaService.VerifyTwoFactorCodeAsync(user.Id, code);
        if (!verified)
            return new LoginResult(false, null, null, null, "Invalid 2FA code.");

        // Generate tokens
        var roles = await _repository.GetUserRolesAsync(user.Id);
        var roleNames = roles.Select(r => r.Name);

        var (accessToken, refreshToken) = _tokenService.GenerateTokenPair(
            user.Id, user.Email, roleNames, [], []);

        user.LastLoginAt = DateTime.UtcNow;
        await _repository.UpdateUserAsync(user);

        return new LoginResult(
            true,
            accessToken,
            refreshToken,
            null,
            "Two factor verified.",
            roleNames.ToList());
    }

    public async Task LogoutAsync(Guid userId)
    {
        await _sessionService.RevokeAllUserSessionsAsync(userId);
    }
}

