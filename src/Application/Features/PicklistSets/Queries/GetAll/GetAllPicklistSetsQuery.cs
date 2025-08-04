// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;


namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Queries.GetAll;

public class GetAllPicklistSetsQuery : ICacheableRequest<IEnumerable<PicklistSetDto>>
{
    public string CacheKey => PicklistSetCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;
}

public class GetAllPicklistSetsQueryHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper)
    : IRequestHandler<GetAllPicklistSetsQuery, IEnumerable<PicklistSetDto>>
{
    public async Task<IEnumerable<PicklistSetDto>> Handle(GetAllPicklistSetsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.PicklistSets.OrderBy(x => x.Name).ThenBy(x => x.Value)
            .ProjectTo<PicklistSetDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }
}