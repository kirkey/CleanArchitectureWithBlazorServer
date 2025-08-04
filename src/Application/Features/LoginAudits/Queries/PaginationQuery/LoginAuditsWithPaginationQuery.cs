// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.Caching;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.Specifications;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.PaginationQuery;

public class LoginAuditsWithPaginationQuery : LoginAuditAdvancedFilter, ICacheableRequest<PaginatedData<LoginAuditDto>>
{
    public LoginAuditAdvancedSpecification Specification => new(this);

    public string CacheKey => LoginAuditCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => LoginAuditCacheKey.Tags;

    public override string ToString()
    {
        return
            $"ListView:{ListView},Success:{Success},Provider:{Provider},Search:{Keyword},OrderBy:{OrderBy} {SortDirection},PageNumber:{PageNumber},PageSize:{PageSize}";
    }
}

public class LoginAuditsQueryHandler(
    IMapper mapper,
    IApplicationDbContextFactory dbContextFactory)
    : IRequestHandler<LoginAuditsWithPaginationQuery, PaginatedData<LoginAuditDto>>
{
    public async Task<PaginatedData<LoginAuditDto>> Handle(LoginAuditsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.LoginAudits.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<LoginAudit, LoginAuditDto>(request.Specification, request.PageNumber, request.PageSize,
                mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}
