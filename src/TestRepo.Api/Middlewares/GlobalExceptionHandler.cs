using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace TestRepo.Api.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        logger.LogError(
            exception,
            "Could not process a request on {MachineName}. TraceId: {TraceId}",
            Environment.MachineName,
            traceId
        );
        var (title, statusCode) = MapException(exception);
        await Results
            .Problem(
                title: title,
                statusCode: statusCode,
                extensions: new Dictionary<string, object?> { { "traceId", traceId } }
            )
            .ExecuteAsync(httpContext)
            .ConfigureAwait(false);
        return true;
    }

    private static (string title, int statusCode) MapException(Exception exception) =>
        exception switch
        {
            _ => ("Internal Server Error", StatusCodes.Status500InternalServerError)
        };
}
