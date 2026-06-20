using FaultLens.Sdk;
using FaultLens.SampleWebApi.Configuration;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace FaultLens.SampleWebApi.Observability;

public sealed class FaultLensExceptionHandler(
    FaultLensClient faultLensClient,
    ILogger<FaultLensExceptionHandler> logger,
    IOptions<FaultLensSampleSettings> sampleSettings)
    : IExceptionHandler
{
    private readonly FaultLensSampleSettings settings = sampleSettings.Value;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var correlationId = httpContext.Request.Headers["X-Correlation-ID"].ToString();
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = httpContext.TraceIdentifier;
        }

        using var requestScope = faultLensClient.BeginRequest(
            httpContext.Request.Method,
            httpContext.Request.Path,
            source: settings.ServiceName,
            data: new Dictionary<string, object>
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["correlationId"] = correlationId,
                ["route"] = httpContext.Request.Path.Value ?? string.Empty,
                ["method"] = httpContext.Request.Method,
                ["environment"] = settings.Environment,
                ["release"] = settings.Release,
                ["service"] = settings.ServiceName,
                ["serviceVersion"] = settings.ServiceVersion
            });

        requestScope.SetAccount(
            accountId: settings.AccountId,
            tenantId: settings.TenantId);
        requestScope.SetUser(settings.UserId);
        requestScope.SetTag("sample", "dotnet");
        requestScope.SetTag("flow", "global-exception-handler");

        faultLensClient.AddDecision(
            category: "sample.unhandled-exception",
            message: "Unhandled exception reached the global exception handler",
            layer: BreadcrumbLayer.Application,
            level: BreadcrumbLevel.Error,
            source: nameof(FaultLensExceptionHandler),
            data: new Dictionary<string, object>
            {
                ["path"] = httpContext.Request.Path.Value ?? string.Empty,
                ["traceId"] = httpContext.TraceIdentifier,
                ["correlationId"] = correlationId,
                ["exceptionType"] = exception.GetType().Name,
                ["exceptionMessage"] = exception.Message,
                ["service"] = settings.ServiceName
            });

        faultLensClient.CaptureException(exception);
        faultLensClient.Flush(TimeSpan.FromSeconds(2));

        logger.LogError(exception, "Unhandled exception captured by FaultLens sample.");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            message = "Unhandled exception captured and forwarded to FaultLens.",
            traceId = httpContext.TraceIdentifier,
            correlationId,
            exception = exception.GetType().Name
        }, cancellationToken);

        requestScope.Fail(StatusCodes.Status500InternalServerError);
        return true;
    }
}
