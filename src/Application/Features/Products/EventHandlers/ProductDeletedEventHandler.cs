// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers;

public class ProductDeletedEventHandler(ILogger<ProductDeletedEventHandler> logger)
    : INotificationHandler<DeletedEvent<Product>>
{
    public Task Handle(DeletedEvent<Product> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ",
            notification.GetType().Name, notification);
        return Task.CompletedTask;
    }
}