// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.Delete;

public class DeleteDocumentCommand(int[] id) : ICacheInvalidatorRequest<Result>
{
    public int[] Id { get; set; } = id;
    public IEnumerable<string>? Tags => DocumentCacheKey.Tags;
}

public class DeleteDocumentCommandHandler(IApplicationDbContextFactory dbContextFactory)
    : IRequestHandler<DeleteDocumentCommand, Result>

{
    public async Task<Result> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var items = await db.Documents.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            item.AddDomainEvent(new DeletedEvent<Document>(item));
            db.Documents.Remove(item);
        }

        await db.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}