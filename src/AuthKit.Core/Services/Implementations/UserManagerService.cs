namespace AuthKit.Core.Services.Implementations;

using AuthKit.Core.Models;
using AuthKit.Core.Services;
using AuthKit.Core.Services.DTOs;

public class UserManagerService : IUserManagerService
{
    public Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<UserDto?>(null);
    }

    public Task<UserDto?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return Task.FromResult<UserDto?>(null);
    }

    public Task<UserDto?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        return Task.FromResult<UserDto?>(null);
    }

    public Task<IEnumerable<UserDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        return Task.FromResult<IEnumerable<UserDto>>(new List<UserDto>());
    }

    public Task<IResult> UpdateAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> ChangeEmailAsync(Guid userId, ChangeEmailRequest request, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> LockAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<IResult> UnlockAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }

    public Task<IResult> DeleteAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult<IResult>(new Result(true, "Not implemented.", null));
    }
}
