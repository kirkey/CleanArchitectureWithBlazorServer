// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class CacheInvalidationBehaviour<TRequest, TResponse>(
    IFusionCache cache,
    ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatorRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogTrace("Handling request of type {RequestType} with details {@Request}", nameof(request), request);
        var response = await next().ConfigureAwait(false);
        if (!string.IsNullOrEmpty(request.CacheKey))
        {
            await cache.RemoveAsync(request.CacheKey, token: cancellationToken);
            logger.LogTrace("Cache key {CacheKey} removed from cache", request.CacheKey);
        }

        if (request.Tags == null || !request.Tags.Any()) return response;
        foreach (var tag in request.Tags)
        {
            await cache.RemoveByTagAsync(tag, token: cancellationToken);
            logger.LogTrace("Cache tag {CacheTag} removed from cache", tag);
        }

        return response;
    }
}