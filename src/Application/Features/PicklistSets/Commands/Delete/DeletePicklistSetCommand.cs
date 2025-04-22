// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.Delete;

public class DeletePicklistSetCommand(int[] id) : ICacheInvalidatorRequest<Result>
{
    public int[] Id { get; } = id;
    public string CacheKey => PicklistSetCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;
}

public class DeletePicklistSetCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeletePicklistSetCommand, Result>

{
    public async Task<Result> Handle(DeletePicklistSetCommand request, CancellationToken cancellationToken)
    {
        var items = await context.PicklistSets.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            var changeEvent = new UpdatedEvent<PicklistSet>(item);
            item.AddDomainEvent(changeEvent);
            context.PicklistSets.Remove(item);
        }

        await context.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}