namespace AuthKit.Core.Services.DTOs;

public record LoginAttemptRequest(string Identifier, string AttemptType, string? IpAddress, bool Succeeded, Guid? UserId);

public record LoginAttemptDto(Guid Id, string Identifier, string AttemptType, string? IpAddress, DateTime AttemptedAt, bool Succeeded);
