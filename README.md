# FaultLens .NET Samples

Public sample applications for integrating FaultLens SDKs into .NET applications.

## Current sample

- `samples/FaultLens.SampleWebApi`

This sample demonstrates a realistic ASP.NET Core integration using `FaultLens.SDK` with:

- `FaultLens.SDK` `1.1.1`
- DI registration for `FaultLensClient`
- request-scoped breadcrumbs via `BeginRequest(...)`
- manual breadcrumbs via `AddStep(...)` and `AddDecision(...)`
- diagnostics context via `SetRequestContext(...)`, `SetAccount(...)`, `SetUser(...)`, `SetAnonymousId(...)`, request-scope data, and `SetTag(...)`
- direct environment, release, service, account, user, anonymous ID, route, method, status, trace, and correlation sample context where the current SDK supports it
- explicit business-severity metadata via `SetCapability(capability, criticality, operation)`
- manual message capture
- handled exception capture
- uncaught exception capture through a global exception handler
- Docker-based local run support

## First event in under 10 minutes

No source edits required — configuration comes from a local `.env` file.

1. Copy the example config and fill in the two required values from your FaultLens project setup
   (`FaultLens__ApiKey` and `FaultLens__Endpoint`):

   ```bash
   # macOS / Linux / Git Bash
   cp .env.example .env
   ```

   ```powershell
   # Windows PowerShell
   Copy-Item .env.example .env
   ```

   Then open `.env` in your editor and set `FaultLens__ApiKey` and `FaultLens__Endpoint`.

2. Start the sample with Docker Compose:

   ```bash
   docker compose up --build
   ```

3. Trigger a sample event (no need to write broken code yourself) — open
   <http://localhost:8080/api/faultlens/critical-capability> in a browser, or:

   ```bash
   # macOS / Linux / Git Bash
   curl http://localhost:8080/api/faultlens/critical-capability
   ```

   ```powershell
   # Windows PowerShell
   Invoke-RestMethod http://localhost:8080/api/faultlens/critical-capability
   ```

4. Open FaultLens and confirm the event appears with `capability = checkout`,
   `criticality = critical`, and `operation = payment-capture` promoted onto the issue.

`.env` is gitignored, so your key is never committed. Only `.env.example` is tracked.

## Prerequisites

- .NET SDK 10.0+
- Docker Desktop (optional, for container run)
- a FaultLens project API key
- your FaultLens ingestion host (shown in your project setup, e.g. `your-workspace.faultlens.in`)

Install the SDK package directly from NuGet:

```powershell
dotnet add package FaultLens.SDK
```

## Configuration

Set FaultLens settings in one of these ways (no source edits required for options 2–4):

1. `samples/FaultLens.SampleWebApi/appsettings.Development.json`
2. a local `.env` file (used by `docker compose`) — copy `.env.example` to `.env`
3. environment variables
4. Docker environment variables

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
$env:FaultLens__Endpoint = 'https://YOUR-WORKSPACE.faultlens.in'
$env:FaultLens__Environment = 'staging'
$env:FaultLens__Release = 'v1.8.4'
$env:FaultLens__ServiceName = 'checkout-api'
$env:FaultLens__ServiceVersion = 'v1.8.4+sample.1'
$env:FaultLens__TenantId = 'tenant_demo_retail'
$env:FaultLens__AccountId = 'acct_demo_standard'
$env:FaultLens__UserId = 'user_demo_123'
$env:FaultLens__AnonymousId = 'anon_demo_browser_456'
```

Use placeholder or non-sensitive sample values only. Do not hardcode production API keys. Do not commit real project keys, tenant identifiers, customer identifiers, user emails, passwords, payment data, access tokens, cookies, request bodies, authorization headers, connection strings, or private endpoints.

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

The recommended path is `.env` + Docker Compose (see "First event in under 10 minutes"):

```bash
cp .env.example .env   # then edit FaultLens__ApiKey and FaultLens__Endpoint
docker compose up --build
```

To build and run the image directly instead:

```bash
docker build -t faultlens-dotnet-samples .
docker run --rm -p 8080:8080 --env-file .env -e ASPNETCORE_ENVIRONMENT=Development faultlens-dotnet-samples
```

## Sample endpoints

- `GET /api/faultlens`
- `POST /api/faultlens/breadcrumbs/manual`
- `POST /api/faultlens/anonymous-context`
- `POST /api/faultlens/capture-message`
- `POST /api/faultlens/diagnostics-context`
- `GET /api/faultlens/critical-capability`
- `GET /api/faultlens/handled-exception`
- `GET /api/faultlens/http-failure`
- `GET /api/faultlens/uncaught-exception`

## What to verify in FaultLens

1. Trigger `GET /api/faultlens/critical-capability` and confirm the issue is promoted with `capability = checkout`, `criticality = critical`, and `operation = payment-capture`. These three reserved tags are the only business-severity metadata the backend consumes.
2. Trigger `POST /api/faultlens/capture-message` and confirm a message event appears.
3. Trigger `POST /api/faultlens/diagnostics-context` and confirm the event includes request URL, method/route context, user agent/runtime context, direct `tenantId`, direct `accountId`, direct `userId`, direct service context, and tags for custom demo metadata such as `sample`, `feature`, `flow`, and `planTier`.
4. Trigger `POST /api/faultlens/anonymous-context` and confirm the event includes direct `anonymousId` without direct `tenantId`, `accountId`, or `userId`.
5. Trigger `GET /api/faultlens/handled-exception` and confirm breadcrumbs include the request scope and controller decisions.
6. Trigger `GET /api/faultlens/http-failure` and confirm the outbound call breadcrumb is attached.
7. Trigger `GET /api/faultlens/uncaught-exception` and confirm the global exception path is captured.

## Business severity metadata

FaultLens classifies severity from observed impact and never infers business criticality from routes or URLs. The `critical-capability` endpoint sends the three reserved tags the backend consumes:

```csharp
requestScope.SetCapability(
    capability: "checkout",
    criticality: FaultLensCriticality.Critical,
    operation: "payment-capture"); // operation may name a route, workflow, job, or command
```

The sample intentionally does not use `SetOperationCriticality`, `SetWorkflow`, or `SetJob`: those helpers were deprecated in SDK 1.1.1 because the backend does not consume them. See the SDK's `docs/capability-metadata.md`.

## Custom integration (no SDK)

FaultLens is platform-independent. Any runtime that can make an authenticated HTTPS request can send the canonical event envelope directly, without the .NET SDK. This is a first-class integration option — see the backend `docs/ingestion-api.md` for the full contract.

```bash
# macOS / Linux / Git Bash
curl -sS -X POST "$FaultLens__Endpoint/api/events/ingest" \
  -H "Content-Type: application/json" \
  -H "X-API-Key: $FaultLens__ApiKey" \
  -d '{
    "eventId": "11111111-1111-1111-1111-111111111111",
    "timestamp": "2026-07-15T12:34:56.000Z",
    "environment": "production",
    "release": "v1.8.4",
    "platform": "custom",
    "sdk": { "name": "custom-curl", "version": "1.0.0" },
    "exception": { "type": "PaymentCaptureError", "message": "Payment capture failed" },
    "tags": {
      "faultlens.capability": "checkout",
      "faultlens.criticality": "critical",
      "faultlens.operation": "payment-capture"
    }
  }'
```

```powershell
# Windows PowerShell
$body = @{
  eventId     = "11111111-1111-1111-1111-111111111111"
  timestamp   = "2026-07-15T12:34:56.000Z"
  environment = "production"
  release     = "v1.8.4"
  platform    = "custom"
  sdk         = @{ name = "custom-curl"; version = "1.0.0" }
  exception   = @{ type = "PaymentCaptureError"; message = "Payment capture failed" }
  tags        = @{
    "faultlens.capability"  = "checkout"
    "faultlens.criticality" = "critical"
    "faultlens.operation"   = "payment-capture"
  }
} | ConvertTo-Json -Depth 5

Invoke-RestMethod -Method Post -Uri "$env:FaultLens__Endpoint/api/events/ingest" `
  -Headers @{ "X-API-Key" = $env:FaultLens__ApiKey } `
  -ContentType "application/json" -Body $body
```

Every business outcome returns HTTP 200 with a JSON body — inspect `status`. `status: 1` is Ingested (stored); `status: 2` is Dropped (e.g. `reasonCode: "billing_limit_exceeded"`, not stored); `status: 3` is Rejected. HTTP `401` means a missing/invalid key or wrong host; `429`/`5xx` are retryable with backoff. Do not treat every HTTP 200 as accepted.

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

Use non-sensitive identifiers that help your team investigate safely. Prefer stable opaque IDs like `tenant_abc123`, `acct_042`, or `user_123`; avoid names, emails, phone numbers, passwords, payment data, tokens, cookies, secrets, full request bodies, or authorization headers.

## Notes

- No real keys are stored in this repo.
- The diagnostics smoke endpoint uses sample values only and does not capture cookies, authorization headers, request bodies, or secrets.
- Release context should be interpreted as observed after deployment, first seen after deployment, or release-adjacent evidence. Do not treat sample release context as proof that a deployment caused an issue.
- This repo is for sample integrations, not production deployment templates.
