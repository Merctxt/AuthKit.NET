using AuthKit.Core.Services;

namespace AuthKit.Identity.Services;

/// <summary>
/// Placeholder implementation - replace with actual email provider (SendGrid, AWS SES, etc.)
/// </summary>
public class EmailService : IEmailService
{
    public Task SendConfirmationEmailAsync(string email, string token)
    {
        // TODO: Implement with actual email provider
        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string email, string token)
    {
        // TODO: Implement with actual email provider
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string email, string userName)
    {
        // TODO: Implement with actual email provider
        return Task.CompletedTask;
    }

    public Task SendTwoFactorCodeAsync(string email, string code)
    {
        // TODO: Implement with actual email provider
        return Task.CompletedTask;
    }
}
