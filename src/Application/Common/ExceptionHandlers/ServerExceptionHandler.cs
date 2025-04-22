namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class
    ServerExceptionHandler<TRequest, TResponse, TException>(
        ILogger<ServerExceptionHandler<TRequest, TResponse, TException>> logger)
    : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result>
    where TResponse : Result
    where TException : ServerException
{
    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        state.SetHandled((TResponse)Result.Failure(exception.Message));
        logger.LogError(exception, exception.Message);
        return Task.CompletedTask;
    }
}