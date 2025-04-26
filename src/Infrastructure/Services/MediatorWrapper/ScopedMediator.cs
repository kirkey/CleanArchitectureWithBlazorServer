using System.Runtime.CompilerServices;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MediatorWrapper;
using MediatR;

namespace CleanArchitecture.Blazor.Infrastructure.Services.MediatorWrapper;

/// <summary>
/// Represents a scoped mediator that provides methods for sending requests, publishing notifications, and creating streams.
/// </summary>
public class ScopedMediator(IServiceScopeFactory scopeFactory) : IScopedMediator
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return ExecuteWithinScope(mediator => mediator.Send(request, cancellationToken));
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        return ExecuteWithinScope(mediator => mediator.Send(request, cancellationToken));
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        return ExecuteWithinScope(mediator => mediator.Send(request, cancellationToken));
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        return ExecuteWithinScope(mediator => mediator.Publish(notification, cancellationToken));
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return ExecuteWithinScope(mediator => mediator.Publish(notification, cancellationToken));
    }

    public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = scopeFactory.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await foreach (var item in mediator.CreateStream(request, cancellationToken))
            yield return item;
    }

    public async IAsyncEnumerable<object?> CreateStream(object request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = scopeFactory.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await foreach (var item in mediator.CreateStream(request, cancellationToken))
            yield return item;
    }

    private async Task<T> ExecuteWithinScope<T>(Func<IMediator, Task<T>> operation)
    {
        using var scope = scopeFactory.CreateAsyncScope();
        return await operation(scope.ServiceProvider.GetRequiredService<IMediator>());
    }

    private Task ExecuteWithinScope(Func<IMediator, Task> operation)
    {
        using var scope = scopeFactory.CreateAsyncScope();
        return operation(scope.ServiceProvider.GetRequiredService<IMediator>());
    }
}