// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Documents.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.Upload;

public class UploadDocumentCommand(List<UploadRequest> uploadRequests) : ICacheInvalidatorRequest<Result<int>>
{
    public List<UploadRequest> UploadRequests { get; set; } = uploadRequests;
    public IEnumerable<string>? Tags => DocumentCacheKey.Tags;
}

public class UploadDocumentCommandHandler(
    IApplicationDbContextFactory dbContextFactory,
    IUploadService uploadService)
    : IRequestHandler<UploadDocumentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var list = new List<Document>();
        foreach (var uploadRequest in request.UploadRequests)
        {
            var fileName = uploadRequest.FileName;
            var url = await uploadService.UploadAsync(uploadRequest);
            var document = new Document
            {
                Title = fileName,
                Url = url,
                Status = JobStatus.Queueing,
                IsPublic = true,
                DocumentType = DocumentType.Image
            };
            document.AddDomainEvent(new CreatedEvent<Document>(document));
            list.Add(document);
        }

        if (!list.Any()) return await Result<int>.SuccessAsync(0);
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        await db.Documents.AddRangeAsync(list, cancellationToken);
        var result = await db.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }
}