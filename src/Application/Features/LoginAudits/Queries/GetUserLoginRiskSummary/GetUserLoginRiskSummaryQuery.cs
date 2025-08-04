// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.LoginAudits.Caching;
using CleanArchitecture.Blazor.Application.Features.LoginAudits.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Queries.GetUserLoginRiskSummary;

public class GetUserLoginRiskSummaryQuery : ICacheableRequest<UserLoginRiskSummaryDto?>
{
    public required string UserId { get; set; }

    public string CacheKey => $"UserLoginRiskSummary_{UserId}";

    public IEnumerable<string>? Tags => LoginAuditCacheKey.Tags;
}

public class GetUserLoginRiskSummaryQueryHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper)
    : IRequestHandler<GetUserLoginRiskSummaryQuery, UserLoginRiskSummaryDto?>
{
    public async Task<UserLoginRiskSummaryDto?> Handle(GetUserLoginRiskSummaryQuery request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var summary = await db.UserLoginRiskSummaries
            .Where(x => x.UserId == request.UserId)
            .ProjectTo<UserLoginRiskSummaryDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
        return summary;
    }
} 