namespace AuthKit.Core.Services.DTOs;

public record UpdateProfileRequest(string? Email = null, string? Username = null);

public record UpdatePasswordRequest(string CurrentPassword, string NewPassword);

public record ChangeEmailRequest(string NewEmail);
