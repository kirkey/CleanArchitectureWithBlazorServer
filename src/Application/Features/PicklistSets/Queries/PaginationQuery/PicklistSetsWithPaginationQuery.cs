// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Queries.PaginationQuery;

public class PicklistSetsWithPaginationQuery : PicklistSetAdvancedFilter,
    ICacheableRequest<PaginatedData<PicklistSetDto>>
{
    public PicklistSetAdvancedSpecification Specification => new(this);
    public string CacheKey => $"{nameof(PicklistSetsWithPaginationQuery)},{this}";
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;

    public override string ToString()
    {
        return
            $"ListView:{ListView}-{Picklist}-{CurrentUser?.UserId},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class PicklistSetsQueryHandler(
    IMapper mapper,
    IApplicationDbContext context) : IRequestHandler<PicklistSetsWithPaginationQuery, PaginatedData<PicklistSetDto>>
{
    public async Task<PaginatedData<PicklistSetDto>> Handle(PicklistSetsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await context.PicklistSets.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<PicklistSet, PicklistSetDto>(request.Specification, request.PageNumber,
                request.PageSize, mapper.ConfigurationProvider, cancellationToken);

        return data;
    }
}