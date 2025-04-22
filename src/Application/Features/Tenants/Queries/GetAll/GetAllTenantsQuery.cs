// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;
using CleanArchitecture.Blazor.Application.Features.Tenants.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Queries.GetAll;

public abstract class GetAllTenantsQuery : ICacheableRequest<IEnumerable<TenantDto>>
{
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => TenantCacheKey.Tags;
}

public class GetAllTenantsQueryHandler(
    IMapper mapper,
    IApplicationDbContext context) :
    IRequestHandler<GetAllTenantsQuery, IEnumerable<TenantDto>>
{
    public async Task<IEnumerable<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        var data = await context.Tenants
            .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }
}