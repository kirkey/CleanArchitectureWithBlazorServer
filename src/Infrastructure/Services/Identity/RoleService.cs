using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class RoleService(
    IMapper mapper,
    IFusionCache fusionCache,
    IServiceScopeFactory scopeFactory)
    : IRoleService, IDisposable
{
    private const string CACHEKEY = "ALL-ApplicationRoleDto";

    public List<ApplicationRoleDto> DataSource { get; private set; } = new();

    public event Func<Task>? OnChange;

    public async Task InitializeAsync()
    {
        DataSource = await fusionCache.GetOrSetAsync(CACHEKEY,
                         async _ =>
                         {
                             using var scope = scopeFactory.CreateScope();
                             var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                             return await roleManager.Roles
                                 .ProjectTo<ApplicationRoleDto>(mapper.ConfigurationProvider)
                                 .OrderBy(x => x.TenantId)
                                 .ThenBy(x => x.Name)
                                 .ToListAsync();
                         })
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }


    public async Task RefreshAsync()
    {
        fusionCache.Remove(CACHEKEY);
        DataSource = await fusionCache.GetOrSetAsync(CACHEKEY,
                         async _ =>
                         {
                             using var scope = scopeFactory.CreateScope();
                             var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                             return await roleManager.Roles
                                 .ProjectTo<ApplicationRoleDto>(mapper.ConfigurationProvider)
                                 .OrderBy(x => x.TenantId)
                                 .ThenBy(x => x.Name)
                                 .ToListAsync();
                         })
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }

    public void Dispose()
    {
        // No long-lived resources to dispose
        GC.SuppressFinalize(this);
    }
}