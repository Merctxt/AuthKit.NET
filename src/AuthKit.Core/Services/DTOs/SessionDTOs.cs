namespace AuthKit.Core.Services.DTOs;

public record SessionDto(Guid Id, Guid UserId, string DeviceInfo, DateTime ExpiresAt, DateTime CreatedAt, bool IsRevoked);

public record CreateSessionRequest(Guid UserId, string DeviceInfo, string RefreshTokenHash, Guid RefreshTokenJti, DateTime ExpiresAt);
