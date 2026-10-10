using Microsoft.Extensions.Options;
using AuthKit.Core.Models;
using AuthKit.Core.Options;
using AuthKit.Core.Services;
using AuthKit.Interfaces.Repositories;

namespace AuthKit.Identity.Services;

public class PasswordService : IPasswordService
{
    private readonly IAuthRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly RateLimitingOptions _rateLimitOptions;

    public PasswordService(
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

    public async Task<PasswordResult> ForgotPasswordAsync(string email)
    {
        var tenant = await _repository.GetDefaultTenantAsync();
        if (tenant == null)
            return new PasswordResult(false, "No tenant configured.");

        var user = await _repository.GetUserByEmailAsync(email, tenant.Id);
        if (user == null)
            return new PasswordResult(true, "If the email exists, a reset link has been sent.");

        var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        await _emailService.SendPasswordResetEmailAsync(email, resetToken);

        return new PasswordResult(true, "If the email exists, a reset link has been sent.");
    }

    public async Task<PasswordResult> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var tenant = await _repository.GetDefaultTenantAsync();
        if (tenant == null)
            return new PasswordResult(false, "No tenant configured.");

        var user = await _repository.GetUserByEmailAsync(email, tenant.Id);
        if (user == null)
            return new PasswordResult(false, "User not found.");

        // Validate token (simplified - in production should use a proper token store)
        if (string.IsNullOrEmpty(token))
            return new PasswordResult(false, "Invalid token.");

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        await _repository.UpdateUserAsync(user);

        return new PasswordResult(true, "Password reset successful.");
    }
}
