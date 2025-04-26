// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Application.Pipeline.PreProcessors;

public class LoggingPreProcessor<TRequest>(ILogger<TRequest> logger, ICurrentUserAccessor currentUserAccessor)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger = logger;


    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = nameof(TRequest);
        var userName = currentUserAccessor.SessionInfo?.UserName;
        _logger.LogTrace("Processing request of type {RequestName} with details {@Request} by user {UserName}",
            requestName, request, userName);
        return Task.CompletedTask;
    }
}