using FaultLens.Sdk;
using Microsoft.AspNetCore.Diagnostics;

namespace FaultLens.SampleWebApi.Observability;

public sealed class FaultLensExceptionHandler(FaultLensClient faultLensClient, ILogger<FaultLensExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        using var requestScope = faultLensClient.BeginRequest(
            httpContext.Request.Method,
            httpContext.Request.Path,
            source: "FaultLens.SampleWebApi",
            data: new Dictionary<string, object>
            {
                ["traceId"] = httpContext.TraceIdentifier
            });

        faultLensClient.AddDecision(
            category: "sample.unhandled-exception",
            message: "Unhandled exception reached the global exception handler",
            layer: BreadcrumbLayer.Application,
            level: BreadcrumbLevel.Error,
            source: nameof(FaultLensExceptionHandler),
            data: new Dictionary<string, object>
            {
                ["path"] = httpContext.Request.Path.Value ?? string.Empty,
                ["traceId"] = httpContext.TraceIdentifier
            });

        faultLensClient.CaptureException(exception);
        faultLensClient.Flush(TimeSpan.FromSeconds(2));

        logger.LogError(exception, "Unhandled exception captured by FaultLens sample.");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            message = "Unhandled exception captured and forwarded to FaultLens.",
            traceId = httpContext.TraceIdentifier,
            exception = exception.GetType().Name
        }, cancellationToken);

        requestScope.Fail(StatusCodes.Status500InternalServerError);
        return true;
    }
}
