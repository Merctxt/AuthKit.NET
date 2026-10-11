using AuthKit.Core.Services.DTOs;

namespace AuthKit.Core.Services;

public record EnableTwoFactorRequest(string Password);

public record TwoFactorResponse(bool Success, string Message, string? QrCodeUrl = null, string? Secret = null, List<string>? BackupCodes = null);

public record VerifyTwoFactorRequest(string Code);

public record DisableTwoFactorRequest(string Password);
