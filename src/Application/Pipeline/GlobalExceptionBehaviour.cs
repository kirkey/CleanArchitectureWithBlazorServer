// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class GlobalExceptionBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    IUserContextAccessor userContextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            var userName = userContextAccessor.Current?.UserName;
            logger.LogError(ex,
                "Request: {RequestName} by User: {UserName} failed. Error: {ErrorMessage}. Request Details: {@Request}",
                requestName,
                userName,
                ex.Message,
                request);
            throw;
        }
    }
}