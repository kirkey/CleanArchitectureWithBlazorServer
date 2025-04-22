using System.Net;

namespace CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;

public class ServerException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    : Exception(message)
{
    public IEnumerable<string> ErrorMessages { get; } = new[] { message };

    public HttpStatusCode StatusCode { get; } = statusCode;
}