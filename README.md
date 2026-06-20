# FaultLens .NET Samples

Public sample applications for integrating FaultLens SDKs into .NET applications.

## Current sample

- `samples/FaultLens.SampleWebApi`

This sample demonstrates a realistic ASP.NET Core integration using `FaultLens.SDK` with:

- `FaultLens.SDK` `1.0.1`
- DI registration for `FaultLensClient`
- request-scoped breadcrumbs via `BeginRequest(...)`
- manual breadcrumbs via `AddStep(...)` and `AddDecision(...)`
- diagnostics context via `SetRequestContext(...)`, `SetAccount(...)`, `SetUser(...)`, `SetAnonymousId(...)`, request-scope data, and `SetTag(...)`
- direct environment, release, service, account, user, anonymous ID, route, method, status, trace, and correlation sample context where the current SDK supports it
- manual message capture
- handled exception capture
- uncaught exception capture through a global exception handler
- Docker-based local run support

## Prerequisites

- .NET SDK 10.0+
- Docker Desktop (optional, for container run)
- a FaultLens project API key
- your tenant FaultLens host

For current staging validation, use your tenant host directly, for example:

```text
https://TENANT-SLUG.staging.faultlens.in
```

## Configuration

Set FaultLens settings in one of these ways:

1. `samples/FaultLens.SampleWebApi/appsettings.Development.json`
2. environment variables
3. Docker environment variables

Required values:

- `FaultLens__ApiKey`
- `FaultLens__Endpoint`
- `FaultLens__Environment`
- `FaultLens__Release`

Optional sample context values:

- `FaultLens__ServiceName`
- `FaultLens__ServiceVersion`
- `FaultLens__TenantId`
- `FaultLens__AccountId`
- `FaultLens__UserId`
- `FaultLens__AnonymousId`

Example PowerShell session:

```powershell
$env:FaultLens__ApiKey = 'YOUR_PROJECT_API_KEY'
$env:FaultLens__Endpoint = 'https://TENANT-SLUG.staging.faultlens.in'
$env:FaultLens__Environment = 'staging'
$env:FaultLens__Release = 'v1.8.4'
$env:FaultLens__ServiceName = 'checkout-api'
$env:FaultLens__ServiceVersion = 'v1.8.4+sample.1'
$env:FaultLens__TenantId = 'tenant_demo_retail'
$env:FaultLens__AccountId = 'acct_demo_standard'
$env:FaultLens__UserId = 'user_demo_123'
$env:FaultLens__AnonymousId = 'anon_demo_browser_456'
```

Use placeholder or non-sensitive sample values only. Do not commit real project keys, tenant identifiers, customer identifiers, user emails, access tokens, cookies, request bodies, authorization headers, connection strings, or private endpoints.

## Local run

```bash
dotnet restore faultlens-dotnet-samples.slnx
dotnet run --project samples/FaultLens.SampleWebApi
```

The sample runs on:

```text
http://localhost:5241
```

If launch profile or port changes locally, use the console output port shown by ASP.NET Core.

## Docker run

Build:

```bash
docker build -t faultlens-dotnet-samples .
```

Run:

```bash
docker run --rm -p 8080:8080 ^
  -e ASPNETCORE_ENVIRONMENT=Development ^
  -e FaultLens__ApiKey=YOUR_PROJECT_API_KEY ^
  -e FaultLens__Endpoint=https://TENANT-SLUG.staging.faultlens.in ^
  -e FaultLens__Environment=staging ^
  -e FaultLens__Release=v1.8.4-docker ^
  -e FaultLens__ServiceName=checkout-api ^
  -e FaultLens__ServiceVersion=v1.8.4+docker ^
  -e FaultLens__TenantId=tenant_demo_retail ^
  -e FaultLens__AccountId=acct_demo_standard ^
  -e FaultLens__UserId=user_demo_123 ^
  -e FaultLens__AnonymousId=anon_demo_browser_456 ^
  faultlens-dotnet-samples
```

Or use:

```bash
docker compose up --build
```

## Sample endpoints

- `GET /api/faultlens`
- `POST /api/faultlens/breadcrumbs/manual`
- `POST /api/faultlens/anonymous-context`
- `POST /api/faultlens/capture-message`
- `POST /api/faultlens/diagnostics-context`
- `GET /api/faultlens/handled-exception`
- `GET /api/faultlens/http-failure`
- `GET /api/faultlens/uncaught-exception`

## What to verify in FaultLens

1. Trigger `POST /api/faultlens/capture-message` and confirm a message event appears.
2. Trigger `POST /api/faultlens/diagnostics-context` and confirm the event includes request URL, method/route context, user agent/runtime context, direct `tenantId`, direct `accountId`, direct `userId`, direct service context, and tags for custom demo metadata such as `sample`, `feature`, `flow`, and `planTier`.
3. Trigger `POST /api/faultlens/anonymous-context` and confirm the event includes direct `anonymousId` without direct `tenantId`, `accountId`, or `userId`.
4. Trigger `GET /api/faultlens/handled-exception` and confirm breadcrumbs include the request scope and controller decisions.
5. Trigger `GET /api/faultlens/http-failure` and confirm the outbound call breadcrumb is attached.
6. Trigger `GET /api/faultlens/uncaught-exception` and confirm the global exception path is captured.

## Context support audit

Supported today:

- `environment`: first-class SDK option through `FaultLensOptions`.
- `release/version`: first-class SDK option through `FaultLensOptions`.
- `service name` and `service version`: first-class SDK options through `FaultLensOptions`.
- `tenant/account/user context`: supported through `SetAccount(...)` and `SetUser(...)`.
- `anonymous id`: supported through `SetAnonymousId(...)`.
- `route`: supported through `BeginRequest(method, route, ...)` and request-scope data.
- `HTTP method`: supported through `BeginRequest(method, route, ...)` and request-scope data.
- `HTTP status`: supported through `requestScope.Complete(statusCode)` and `requestScope.Fail(statusCode)`.
- `exception type`, `exception message`, and `stack trace`: supported by `CaptureException(...)`; the sample also records safe exception type/message breadcrumbs.
- `request id / correlation id`: supported as safe request-scope data and breadcrumb metadata. The sample reads `X-Correlation-ID` when provided and falls back to `HttpContext.TraceIdentifier`.
- `safe custom tags/metadata`: supported by `SetTag(...)`, request-scope data, and breadcrumb data. Tags are for extra metadata, not primary service/account/user identity.
- `user id`: supported by `SetUser(...)`; the sample uses a placeholder non-email demo value.

Partially supported:

- ASP.NET Core HTTP header capture is explicit in this sample. The SDK does not install automatic middleware.

Unsupported / backend or SDK follow-up:

- Automatic ASP.NET Core middleware/DI integration for request/header capture.
- A public contract for richer release-adjacent deployment evidence beyond the current `release` string.

Unsupported / sample-only gap:

- None required for this pass. The sample now emits every requested field that can be represented honestly by the current SDK surface.

## How FaultLens uses this context

- Overview and project health views can group signals by environment, service, release, and account-style context.
- Issues and Events can show route, method, status, user, correlation, breadcrumb, and exception evidence without frontend mock data.
- Releases can identify events first seen after deployment or release-adjacent clusters when the configured `release` value changes.
- Alerting and environment reporting can filter by stable environment labels such as `production`, `staging`, and `development`.

Use non-sensitive identifiers that help your team investigate safely. Prefer stable opaque IDs like `tenant_abc123`, `acct_042`, or `user_123`; avoid names, emails, phone numbers, tokens, cookies, secrets, full request bodies, or authorization headers.

## Notes

- No real keys are stored in this repo.
- The diagnostics smoke endpoint uses sample values only and does not capture cookies, authorization headers, request bodies, or secrets.
- Release context should be interpreted as observed after deployment, first seen after deployment, or release-adjacent evidence. Do not treat sample release context as proof that a deployment caused an issue.
- This repo is for sample integrations, not production deployment templates.
