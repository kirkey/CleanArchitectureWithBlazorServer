// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.Caching;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.GetAll;

public class GetAllLoginAuditsQuery : ICacheableRequest<IEnumerable<LoginAuditDto>>
{
    public string CacheKey => LoginAuditCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => LoginAuditCacheKey.Tags;
}

public class GetAllLoginAuditsQueryHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper)
    : IRequestHandler<GetAllLoginAuditsQuery, IEnumerable<LoginAuditDto>>
{
    public async Task<IEnumerable<LoginAuditDto>> Handle(GetAllLoginAuditsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.LoginAudits
            .OrderByDescending(x => x.LoginTimeUtc)
            .Take(1000) // Limit to latest 1000 records for performance
            .ProjectTo<LoginAuditDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        return data;
    }
}
