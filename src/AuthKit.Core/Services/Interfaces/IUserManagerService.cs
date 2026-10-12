namespace AuthKit.Core.Services;

using AuthKit.Core.Models;
using AuthKit.Core.Services.DTOs;

public interface IUserManagerService
{
    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken ct = default);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<UserDto?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<IEnumerable<UserDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<IResult> UpdateAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default);
    Task<IResult> ChangeEmailAsync(Guid userId, ChangeEmailRequest request, CancellationToken ct = default);
    Task<IResult> LockAsync(Guid userId, CancellationToken ct = default);
    Task<IResult> UnlockAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);
    Task<IResult> DeleteAsync(Guid userId, CancellationToken ct = default);
}
