using CleanArchitecture.Blazor.Application.Features.Identity.DTOs;
using CleanArchitecture.Blazor.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services.Identity;

public class UserService(
    IMapper mapper,
    IFusionCache fusionCache,
    IServiceScopeFactory scopeFactory)
    : IUserService, IDisposable
{
    private const string CACHEKEY = "ALL-ApplicationUserDto";

    public List<ApplicationUserDto> DataSource { get; private set; } = new();

    public event Func<Task>? OnChange;

    public async Task InitializeAsync()
    {
        DataSource = await fusionCache.GetOrSetAsync(CACHEKEY,
                         async _ =>
                         {
                             using var scope = scopeFactory.CreateScope();
                             var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                             return await userManager.Users
                                 .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                                 .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                                 .OrderBy(x => x.UserName)
                                 .ToListAsync();
                         })
                     ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }


    public async Task RefreshAsync()
    {
        fusionCache.Remove(CACHEKEY);
        DataSource = await fusionCache.GetOrSetAsync(CACHEKEY,
                         async _ =>
                         {
                             using var scope = scopeFactory.CreateScope();
                             var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                             return await userManager.Users
                                 .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                                 .ProjectTo<ApplicationUserDto>(mapper.ConfigurationProvider)
                                 .OrderBy(x => x.UserName)
                                 .ToListAsync();
                         })
                     ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }

    public void Dispose()
    {
        // No long-lived resources to dispose
        GC.SuppressFinalize(this);
    }
}