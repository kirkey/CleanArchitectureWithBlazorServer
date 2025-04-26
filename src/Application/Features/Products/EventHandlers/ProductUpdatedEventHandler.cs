// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers;

public class ProductUpdatedEventHandler(ILogger<ProductUpdatedEventHandler> logger)
    : INotificationHandler<UpdatedEvent<Product>>
{
    public Task Handle(UpdatedEvent<Product> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ",
            notification.GetType().Name, notification);

        return Task.CompletedTask;
    }
}