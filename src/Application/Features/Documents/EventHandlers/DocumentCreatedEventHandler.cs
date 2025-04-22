// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentCreatedEventHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<DocumentCreatedEventHandler> logger)
    : INotificationHandler<CreatedEvent<Document>>
{
    public Task Handle(CreatedEvent<Document> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Document upload successful. Beginning OCR recognition process for Document Id: {DocumentId}",
            notification.Entity.Id);

        using (var scope = scopeFactory.CreateScope())
        {
            var ocrJob = scope.ServiceProvider.GetRequiredService<IDocumentOcrJob>();
            BackgroundJob.Enqueue(() => ocrJob.Do(notification.Entity.Id));
        }

        return Task.CompletedTask;
    }
}