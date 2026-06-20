using FaultLens.Sdk;
using FaultLens.SampleWebApi.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace FaultLens.SampleWebApi.Controllers;

[ApiController]
[Route("api/faultlens")]
public sealed class FaultLensDemoController(
    FaultLensClient faultLensClient,
    IHttpClientFactory httpClientFactory,
    IOptions<FaultLensSampleSettings> sampleSettings) : ControllerBase
{
    private readonly FaultLensSampleSettings settings = sampleSettings.Value;

    [HttpGet]
    public IActionResult GetOverview()
    {
        return Ok(new
        {
            sample = "FaultLens.SampleWebApi",
            endpoints = new[]
            {
                "GET /api/faultlens",
                "POST /api/faultlens/breadcrumbs/manual",
                "POST /api/faultlens/anonymous-context",
                "POST /api/faultlens/capture-message",
                "POST /api/faultlens/diagnostics-context",
                "GET /api/faultlens/handled-exception",
                "GET /api/faultlens/http-failure",
                "GET /api/faultlens/uncaught-exception"
            },
            demoContext = new
            {
                environment = settings.Environment,
                release = settings.Release,
                service = settings.ServiceName,
                serviceVersion = settings.ServiceVersion,
                tenantId = settings.TenantId,
                accountId = settings.AccountId,
                userId = settings.UserId,
                anonymousId = settings.AnonymousId
            },
            notes = "Use these endpoints to exercise FaultLens breadcrumbs, handled failures, and unhandled exception capture."
        });
    }

    [HttpPost("diagnostics-context")]
    public async Task<IActionResult> DiagnosticsContextAsync()
    {
        using var requestScope = BeginRequestScope("POST", "/api/faultlens/diagnostics-context");

        ApplyDiagnosticsContext(requestScope);

        faultLensClient.AddStep(
            category: "sample.diagnostics-context.entry",
            message: "Diagnostics context smoke endpoint invoked",
            layer: BreadcrumbLayer.Application,
            source: nameof(FaultLensDemoController),
            data: new Dictionary<string, object>
            {
                ["traceId"] = HttpContext.TraceIdentifier,
                ["correlationId"] = CorrelationId,
                ["service"] = settings.ServiceName,
                ["serviceVersion"] = settings.ServiceVersion
            });

        var result = await CaptureWithDeliveryResultAsync(callback =>
            faultLensClient.CaptureMessage(
                "FaultLens .NET sample diagnostics context capture",
                fingerprint: "dotnet-sample:diagnostics-context",
                callback: callback));

        requestScope.Complete(StatusCodes.Status200OK);
        return Ok(new
        {
            captured = true,
            delivery = result,
            identity = new
            {
                tenantId = settings.TenantId,
                accountId = settings.AccountId,
                userId = settings.UserId
            },
            tags = new Dictionary<string, string>
            {
                ["sample"] = "dotnet",
                ["feature"] = "diagnostics-context",
                ["flow"] = "manual-smoke-test"
            },
            correlationId = CorrelationId,
            traceId = HttpContext.TraceIdentifier
        });
    }

    [HttpPost("breadcrumbs/manual")]
    public IActionResult AddManualBreadcrumb()
    {
        using var requestScope = BeginRequestScope("POST", "/api/faultlens/breadcrumbs/manual");

        faultLensClient.AddStep(
            category: "sample.manual-breadcrumb",
            message: "Manual breadcrumb endpoint invoked",
            layer: BreadcrumbLayer.Application,
            source: nameof(FaultLensDemoController),
            data: new Dictionary<string, object>
            {
                ["traceId"] = HttpContext.TraceIdentifier,
                ["correlationId"] = CorrelationId,
                ["route"] = "/api/faultlens/breadcrumbs/manual",
                ["method"] = "POST"
            });

        requestScope.Complete(StatusCodes.Status200OK);
        return Ok(new
        {
            breadcrumbAdded = true,
            correlationId = CorrelationId,
            traceId = HttpContext.TraceIdentifier
        });
    }

    [HttpPost("anonymous-context")]
    public async Task<IActionResult> AnonymousContextAsync()
    {
        using var requestScope = BeginRequestScope(
            "POST",
            "/api/faultlens/anonymous-context",
            applyKnownIdentity: false);

        requestScope.SetAnonymousId(settings.AnonymousId);
        requestScope.SetCorrelationId(CorrelationId);
        requestScope.SetTag("feature", "anonymous-context");
        requestScope.SetTag("flow", "anonymous-smoke-test");

        faultLensClient.AddStep(
            category: "sample.anonymous-context.entry",
            message: "Anonymous context smoke endpoint invoked",
            layer: BreadcrumbLayer.Application,
            source: nameof(FaultLensDemoController),
            data: new Dictionary<string, object>
            {
                ["traceId"] = HttpContext.TraceIdentifier,
                ["correlationId"] = CorrelationId,
                ["route"] = "/api/faultlens/anonymous-context",
                ["method"] = "POST"
            });

        var result = await CaptureWithDeliveryResultAsync(callback =>
            faultLensClient.CaptureMessage(
                "FaultLens .NET sample anonymous context capture",
                fingerprint: "dotnet-sample:anonymous-context",
                callback: callback));

        requestScope.Complete(StatusCodes.Status200OK);
        return Ok(new
        {
            captured = true,
            delivery = result,
            identity = new
            {
                anonymousId = settings.AnonymousId
            },
            tags = new Dictionary<string, string>
            {
                ["sample"] = "dotnet",
                ["feature"] = "anonymous-context",
                ["flow"] = "anonymous-smoke-test"
            },
            correlationId = CorrelationId,
            traceId = HttpContext.TraceIdentifier
        });
    }

    [HttpPost("capture-message")]
    public async Task<IActionResult> CaptureMessageAsync()
    {
        using var requestScope = BeginRequestScope("POST", "/api/faultlens/capture-message");

        faultLensClient.AddStep(
            category: "sample.capture-message.entry",
            message: "Capture message endpoint invoked",
            layer: BreadcrumbLayer.Application,
            source: nameof(FaultLensDemoController));

        faultLensClient.AddDecision(
            category: "sample.capture-message.fingerprint",
            message: "Using a stable fingerprint for sample message capture",
            layer: BreadcrumbLayer.Domain,
            source: nameof(FaultLensDemoController),
            data: new Dictionary<string, object>
            {
                ["fingerprint"] = "dotnet-sample:message",
                ["service"] = settings.ServiceName,
                ["release"] = settings.Release,
                ["environment"] = settings.Environment
            });

        var result = await CaptureWithDeliveryResultAsync(callback =>
            faultLensClient.CaptureMessage(
                "FaultLens .NET sample manual message capture",
                fingerprint: "dotnet-sample:message",
                callback: callback));

        requestScope.Complete(StatusCodes.Status200OK);
        return Ok(new
        {
            captured = true,
            delivery = result,
            correlationId = CorrelationId,
            traceId = HttpContext.TraceIdentifier
        });
    }

    [HttpGet("handled-exception")]
    public async Task<IActionResult> HandledExceptionAsync()
    {
        using var requestScope = BeginRequestScope("GET", "/api/faultlens/handled-exception");

        try
        {
            faultLensClient.AddStep(
                category: "sample.handled-exception.entry",
                message: "Entering handled exception demo",
                layer: BreadcrumbLayer.Application,
                source: nameof(FaultLensDemoController));

            throw new InvalidOperationException("Handled exception from the FaultLens .NET sample.");
        }
        catch (Exception ex)
        {
            faultLensClient.AddDecision(
                category: "sample.handled-exception.catch",
                message: "Exception was caught and will be forwarded to FaultLens",
                layer: BreadcrumbLayer.Domain,
                level: BreadcrumbLevel.Warning,
                source: nameof(FaultLensDemoController),
                data: new Dictionary<string, object>
                {
                    ["exceptionType"] = ex.GetType().Name,
                    ["exceptionMessage"] = ex.Message,
                    ["route"] = "/api/faultlens/handled-exception",
                    ["method"] = "GET",
                    ["service"] = settings.ServiceName
                });

            var result = await CaptureWithDeliveryResultAsync(callback =>
                faultLensClient.CaptureException(ex, fingerprint: "dotnet-sample:handled", callback: callback));

            requestScope.Complete(StatusCodes.Status200OK);
            return Ok(new
            {
                captured = true,
                handled = true,
                delivery = result,
                exception = ex.GetType().Name,
                correlationId = CorrelationId,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpGet("http-failure")]
    public async Task<IActionResult> HttpFailureAsync()
    {
        using var requestScope = BeginRequestScope("GET", "/api/faultlens/http-failure");
        var client = httpClientFactory.CreateClient("demo-upstream");

        faultLensClient.AddStep(
            category: "sample.http-failure.outbound",
            message: "Calling unreachable upstream endpoint to demonstrate external breadcrumb context",
            layer: BreadcrumbLayer.External,
            level: BreadcrumbLevel.Warning,
            source: nameof(FaultLensDemoController),
            data: new Dictionary<string, object>
            {
                ["url"] = "http://localhost:65534/unreachable",
                ["method"] = "GET",
                ["route"] = "/api/faultlens/http-failure",
                ["correlationId"] = CorrelationId,
                ["service"] = settings.ServiceName
            });

        try
        {
            await client.GetAsync("http://localhost:65534/unreachable");
            requestScope.Complete(StatusCodes.Status200OK);
            return Ok(new { reached = true });
        }
        catch (Exception ex)
        {
            var result = await CaptureWithDeliveryResultAsync(callback =>
                faultLensClient.CaptureException(ex, fingerprint: "dotnet-sample:http-failure", callback: callback));

            requestScope.Complete(StatusCodes.Status502BadGateway);
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                captured = true,
                delivery = result,
                exception = ex.GetType().Name,
                upstreamStatus = StatusCodes.Status502BadGateway,
                correlationId = CorrelationId,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpGet("uncaught-exception")]
    public IActionResult UncaughtException()
    {
        using var requestScope = BeginRequestScope("GET", "/api/faultlens/uncaught-exception");

        faultLensClient.AddStep(
            category: "sample.uncaught-exception.entry",
            message: "About to throw an unhandled exception",
            layer: BreadcrumbLayer.Application,
            level: BreadcrumbLevel.Warning,
            source: nameof(FaultLensDemoController));

        requestScope.Fail(StatusCodes.Status500InternalServerError);
        throw new HttpRequestException(
            "Simulated upstream outage from the FaultLens .NET sample.",
            null,
            HttpStatusCode.BadGateway);
    }

    private IFaultLensRequestScope BeginRequestScope(string method, string route, bool applyKnownIdentity = true)
    {
        var scope = faultLensClient.BeginRequest(
            method,
            route,
            source: settings.ServiceName,
            data: new Dictionary<string, object>
            {
                ["traceId"] = HttpContext.TraceIdentifier,
                ["correlationId"] = CorrelationId,
                ["route"] = route,
                ["method"] = method,
                ["environment"] = settings.Environment,
                ["release"] = settings.Release,
                ["service"] = settings.ServiceName,
                ["serviceVersion"] = settings.ServiceVersion
            });

        ApplySampleContext(scope, applyKnownIdentity);
        return scope;
    }

    private void ApplyDiagnosticsContext(IFaultLensRequestScope requestScope)
    {
        requestScope.SetRequestContext(
            HttpContext.Request.GetDisplayUrl(),
            referrer: Request.Headers["Referer"].ToString(),
            userAgent: Request.Headers["User-Agent"].ToString(),
            queryString: Request.QueryString.HasValue
                ? Request.QueryString.Value?.TrimStart('?')
                : null);

        requestScope.SetTag("feature", "diagnostics-context");
        requestScope.SetTag("flow", "manual-smoke-test");
    }

    private void ApplySampleContext(IFaultLensRequestScope requestScope, bool applyKnownIdentity)
    {
        if (applyKnownIdentity)
        {
            requestScope.SetAccount(
                accountId: settings.AccountId,
                tenantId: settings.TenantId);
            requestScope.SetUser(settings.UserId);
        }

        requestScope.SetTag("sample", "dotnet");
        requestScope.SetTag("planTier", "demo-enterprise");
    }

    private string CorrelationId
    {
        get
        {
            var headerValue = Request.Headers["X-Correlation-ID"].ToString();
            return string.IsNullOrWhiteSpace(headerValue)
                ? HttpContext.TraceIdentifier
                : headerValue;
        }
    }

    private async Task<object> CaptureWithDeliveryResultAsync(Action<Action<DeliveryResult>> capture)
    {
        var completionSource = new TaskCompletionSource<DeliveryResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        capture(result => completionSource.TrySetResult(result));
        faultLensClient.Flush(TimeSpan.FromSeconds(2));

        var completedTask = await Task.WhenAny(completionSource.Task, Task.Delay(TimeSpan.FromSeconds(2)));
        if (completedTask != completionSource.Task)
        {
            return new
            {
                success = false,
                errorCode = "timeout",
                errorMessage = "FaultLens delivery callback did not arrive within the sample timeout window."
            };
        }

        var result = await completionSource.Task;
        return new
        {
            success = result.Success,
            errorCode = result.ErrorCode,
            errorMessage = result.ErrorMessage
        };
    }
}
