# FaultLens .NET Samples

Public sample applications for integrating FaultLens SDKs into .NET applications.

## Current sample

- `samples/FaultLens.SampleWebApi`

This sample demonstrates a realistic ASP.NET Core integration using `FaultLens.SDK` with:

- `FaultLens.SDK` `0.1.0-beta.2`
- DI registration for `FaultLensClient`
- request-scoped breadcrumbs via `BeginRequest(...)`
- manual breadcrumbs via `AddStep(...)` and `AddDecision(...)`
- diagnostics context via `SetRequestContext(...)`, `SetUserId(...)`, and `SetTag(...)`
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

Example PowerShell session:

```powershell
$env:FaultLens__ApiKey = 'YOUR_PROJECT_API_KEY'
$env:FaultLens__Endpoint = 'https://TENANT-SLUG.staging.faultlens.in'
$env:FaultLens__Environment = 'staging'
$env:FaultLens__Release = 'faultlens-dotnet-sample-local'
```

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
  -e FaultLens__Release=faultlens-dotnet-sample-docker ^
  faultlens-dotnet-samples
```

Or use:

```bash
docker compose up --build
```

## Sample endpoints

- `GET /api/faultlens`
- `POST /api/faultlens/breadcrumbs/manual`
- `POST /api/faultlens/capture-message`
- `POST /api/faultlens/diagnostics-context`
- `GET /api/faultlens/handled-exception`
- `GET /api/faultlens/http-failure`
- `GET /api/faultlens/uncaught-exception`

## What to verify in FaultLens

1. Trigger `POST /api/faultlens/capture-message` and confirm a message event appears.
2. Trigger `POST /api/faultlens/diagnostics-context` and confirm the event includes request URL, method/route context, user agent/runtime context, `userId = local-demo-user`, and tags for `sample`, `feature`, and `flow`.
3. Trigger `GET /api/faultlens/handled-exception` and confirm breadcrumbs include the request scope and controller decisions.
4. Trigger `GET /api/faultlens/http-failure` and confirm the outbound call breadcrumb is attached.
5. Trigger `GET /api/faultlens/uncaught-exception` and confirm the global exception path is captured.

## Notes

- No real keys are stored in this repo.
- The diagnostics smoke endpoint uses sample values only and does not capture cookies, authorization headers, request bodies, or secrets.
- This repo is for sample integrations, not production deployment templates.
