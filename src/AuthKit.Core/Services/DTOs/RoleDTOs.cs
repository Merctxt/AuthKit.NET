namespace AuthKit.Core.Services.DTOs;

public record RoleRequest(string Name, string? Description = null, List<string>? Permissions = null);

public record RoleDto(Guid Id, string Name, string? Description, List<string> Permissions);
