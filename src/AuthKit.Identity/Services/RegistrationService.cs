using Microsoft.Extensions.Options;
using AuthKit.Core.Models;
using AuthKit.Core.Options;
using AuthKit.Core.Services;
using AuthKit.Interfaces.Repositories;

namespace AuthKit.Identity.Services;

public class RegistrationService : IRegistrationService
{
    private readonly IAuthRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly RateLimitingOptions _rateLimitOptions;

    public RegistrationService(
        IAuthRepository repository,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IOptions<RateLimitingOptions> rateLimitOptions)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _rateLimitOptions = rateLimitOptions.Value;
    }

    public async Task<RegistrationResult> RegisterAsync(string email, string username, string password, string? tenantId = null)
    {
        // Check if user already exists
        var tenant = await _repository.GetDefaultTenantAsync();
        if (tenant == null)
            return new RegistrationResult(false, null, "No tenant configured.", false);

        var effectiveTenantId = string.IsNullOrEmpty(tenantId) ? tenant.Id : Guid.Parse(tenantId);

        if (await _repository.UserExistsByEmailAsync(email, effectiveTenantId))
            return new RegistrationResult(false, null, "Email already registered.", false);

        if (!string.IsNullOrEmpty(username) && await _repository.UserExistsByUsernameAsync(username))
            return new RegistrationResult(false, null, "Username already taken.", false);

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = effectiveTenantId,
            Email = email.ToLowerInvariant(),
            Username = username,
            PasswordHash = _passwordHasher.HashPassword(password),
            EmailConfirmed = false
        };

        await _repository.CreateUserAsync(user);

        // Send confirmation email
        var confirmationToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        await _emailService.SendConfirmationEmailAsync(email, confirmationToken);

        return new RegistrationResult(
            true,
            user.Id,
            "Registration successful. Please check your email to confirm.",
            true);
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var tenant = await _repository.GetDefaultTenantAsync();
        if (tenant == null) return false;

        var user = await _repository.GetUserByEmailAsync(email, tenant.Id);
        if (user == null) return false;

        user.EmailConfirmed = true;
        await _repository.UpdateUserAsync(user);

        return true;
    }
}


