using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class PicklistService(
    IMapper mapper,
    IFusionCache fusionCache,
    IApplicationDbContextFactory dbContextFactory)
    : IPicklistService
{
    public event  Func<Task>? OnChange;
    public List<PicklistSetDto> DataSource { get; private set; } = new();


    public async Task InitializeAsync()
    {
        await using var db = await dbContextFactory.CreateAsync();
        DataSource = fusionCache.GetOrSet(PicklistSetCacheKey.PicklistCacheKey,
            _ => db.PicklistSets.ProjectTo<PicklistSetDto>(mapper.ConfigurationProvider)
                .OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ToList()
        ) ?? new List<PicklistSetDto>();
    }

    public async Task RefreshAsync()
    {
        fusionCache.Remove(PicklistSetCacheKey.PicklistCacheKey);
        await using var db = await dbContextFactory.CreateAsync();
        DataSource = fusionCache.GetOrSet(PicklistSetCacheKey.PicklistCacheKey,
            _ => db.PicklistSets.ProjectTo<PicklistSetDto>(mapper.ConfigurationProvider)
                .OrderBy(x => x.Name).ThenBy(x => x.Value)
                .ToList()
        ) ?? new List<PicklistSetDto>();
        if (OnChange != null) await OnChange.Invoke();
    }
}