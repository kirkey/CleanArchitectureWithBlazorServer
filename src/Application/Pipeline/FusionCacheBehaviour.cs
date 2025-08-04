// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Application.Pipeline;

public class FusionCacheBehaviour<TRequest, TResponse>(
    IFusionCache fusionCache,
    ILogger<FusionCacheBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogTrace("Handling request of type {RequestType} with cache key {CacheKey}", nameof(request), request.CacheKey);
        var response = await fusionCache.GetOrSetAsync(
            request.CacheKey,
            _ => next(),
            tags:request.Tags
            ).ConfigureAwait(false);

        return response;
    }
}