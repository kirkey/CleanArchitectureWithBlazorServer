// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.EventHandlers;

public class PicklistSetChangedEventHandler(
    IPicklistService picklistService,
    ILogger<PicklistSetChangedEventHandler> logger)
    : INotificationHandler<UpdatedEvent<PicklistSet>>
{
    public Task Handle(UpdatedEvent<PicklistSet> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handled domain event '{EventType}' with notification: {@Notification} ",
             notification.GetType().Name,
             notification);
        picklistService.Refresh();
        return Task.CompletedTask;
    }
}