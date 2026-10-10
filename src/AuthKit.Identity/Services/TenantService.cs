using AuthKit.Core.Models;
using AuthKit.Core.Services;
using AuthKit.Interfaces.Repositories;

namespace AuthKit.Identity.Services;

public class TenantService : ITenantService
{
    private readonly IAuthRepository _repository;

    public TenantService(IAuthRepository repository)
    {
        _repository = repository;
    }

    public async Task<TenantDto?> GetTenantAsync(Guid id)
    {
        var tenant = await _repository.GetTenantByIdAsync(id);
        return tenant == null ? null : MapToDto(tenant);
    }

    public async Task<TenantDto?> GetTenantByDomainAsync(string domain)
    {
        var tenant = await _repository.GetTenantByDomainAsync(domain);
        return tenant == null ? null : MapToDto(tenant);
    }

    public async Task<TenantDto> GetDefaultTenantAsync()
    {
        var tenant = await _repository.GetDefaultTenantAsync();
        return tenant == null ? throw new InvalidOperationException("No default tenant") : MapToDto(tenant);
    }

    public async Task<bool> UserBelongsToTenantAsync(Guid userId, Guid tenantId)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        return user?.TenantId == tenantId;
    }

    private static TenantDto MapToDto(Tenant tenant) => new(
        tenant.Id,
        tenant.Name,
        tenant.Domain,
        tenant.IsActive);
}
