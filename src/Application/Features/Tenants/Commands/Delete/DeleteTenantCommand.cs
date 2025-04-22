// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Application.Features.Tenants.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Tenants.Commands.Delete;

public class DeleteTenantCommand(string[] id) : ICacheInvalidatorRequest<Result>
{
    public string[] Id { get; } = id;
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => TenantCacheKey.Tags;
}

public class DeleteTenantCommandHandler(
    ITenantService tenantsService,
    IApplicationDbContext context)
    :
        IRequestHandler<DeleteTenantCommand, Result>

{
    public async Task<Result> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        var items = await context.Tenants.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items) context.Tenants.Remove(item);

        await context.SaveChangesAsync(cancellationToken);
        tenantsService.Refresh();
        return await Result.SuccessAsync();
    }
}