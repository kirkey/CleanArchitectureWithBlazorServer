
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MultiTenant;

public class TenantService(
    IMapper mapper,
    IFusionCache fusionCache,
    IApplicationDbContextFactory dbContextFactory)
    : ITenantService
{
    public event Func<Task>? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();


    public async Task InitializeAsync()
    {
        await using var db = await dbContextFactory.CreateAsync();
        DataSource = fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
            _ => db.Tenants.ProjectTo<TenantDto>(mapper.ConfigurationProvider)
                .OrderBy(x => x.Name)
                .ToList()) ?? new List<TenantDto>();
    }

    public async Task RefreshAsync()
    {
        fusionCache.Remove(TenantCacheKey.TenantsCacheKey);
        await using var db = await dbContextFactory.CreateAsync();
        DataSource = fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
            _ => db.Tenants.ProjectTo<TenantDto>(mapper.ConfigurationProvider)
                .OrderBy(x => x.Name)
                .ToList()) ?? new List<TenantDto>();
        if (OnChange != null) await OnChange.Invoke();
    }
}