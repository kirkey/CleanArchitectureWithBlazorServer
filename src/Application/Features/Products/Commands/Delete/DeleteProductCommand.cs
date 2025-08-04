// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Delete;

public class DeleteProductCommand(int[] id) : ICacheInvalidatorRequest<Result>
{
    public int[] Id { get; } = id;
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
}

public class DeleteProductCommandHandler(IApplicationDbContextFactory dbContextFactory) :
    IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var items = await db.Products.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            item.AddDomainEvent(new DeletedEvent<Product>(item));
            db.Products.Remove(item);
        }

        await db.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}