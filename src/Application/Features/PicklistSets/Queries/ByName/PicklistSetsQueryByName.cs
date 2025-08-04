// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;


namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Queries.ByName;

public class PicklistSetsQueryByName(Picklist name) : ICacheableRequest<IEnumerable<PicklistSetDto>>
{
    public Picklist Name { get; set; } = name;
    public string CacheKey => PicklistSetCacheKey.GetCacheKey(Name.ToString());
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;
}

public class PicklistSetsQueryByNameHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper)
    : IRequestHandler<PicklistSetsQueryByName, IEnumerable<PicklistSetDto>>
{
    public async Task<IEnumerable<PicklistSetDto>> Handle(PicklistSetsQueryByName request,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.PicklistSets.Where(x => x.Name == request.Name)
            .OrderBy(x => x.Text)
            .ProjectTo<PicklistSetDto>(mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return data;
    }
}