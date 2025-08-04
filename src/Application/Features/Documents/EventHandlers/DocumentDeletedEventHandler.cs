// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers;

public class DocumentDeletedEventHandler(ILogger<DocumentDeletedEventHandler> logger)
    : INotificationHandler<DeletedEvent<Document>>
{
    public Task Handle(DeletedEvent<Document> notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(notification.Entity.Url))
        {
            logger.LogWarning("The document URL is null or empty, skipping file deletion.");
            return Task.CompletedTask;
        }

        var folder = UploadType.Document.GetDescription();
        var folderName = Path.Combine("Files", folder);
        var deleteFilePath = Path.Combine(Directory.GetCurrentDirectory(), folderName, notification.Entity.Url);

        if (File.Exists(deleteFilePath))
        {
            try
            {
                File.Delete(deleteFilePath);
                logger.LogInformation("File deleted successfully: {FilePath}", deleteFilePath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete file: {FilePath}", deleteFilePath);
            }
        }
        else
        {
            logger.LogWarning("File not found for deletion: {FilePath}", deleteFilePath);
        }

        return Task.CompletedTask;
    }
}