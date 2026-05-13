# CLAUDE.md — faultlens-dotnet-samples (ClaudeCode)

> **Read [AGENTS.md](AGENTS.md) first.** It is the canonical shared rule file covering sample purpose, work mode, GitHub tracking, branch rules, configuration/secret safety, SDK version, target framework, sample flow preservation, Docker conventions, and validation expectations. This file contains only ClaudeCode-specific notes.

---

FaultLens .NET Samples — ASP.NET Core WebApi sample demonstrating FaultLens .NET SDK integration.

## Layout

```
samples/
  FaultLens.SampleWebApi/
    Controllers/          Demo endpoints (FaultLensDemoController)
    Configuration/        FaultLensSampleSettings — typed config binding
    Observability/        FaultLensExceptionHandler — global unhandled exception capture
    Program.cs            DI registration, FaultLensClient singleton setup
    appsettings.json      base config (no secrets)
    appsettings.Development.json  local overrides (gitignored secrets)
faultlens-dotnet-samples.slnx   solution file
Dockerfile              multi-stage build
docker-compose.yml      local Docker run
README.md               run instructions and integration guide
```

## Stack

| Layer | Choice |
|---|---|
| Framework | ASP.NET Core (net10.0) |
| FaultLens SDK | `FaultLens.SDK` v0.1.0-beta.2 |
| Language | C# (ImplicitUsings enabled, Nullable enabled) |
| Containerised run | Docker |

## Commands

```bash
dotnet build faultlens-dotnet-samples.slnx    # build solution
dotnet run --project samples/FaultLens.SampleWebApi  # run locally

docker build .                # build container image
docker-compose up             # run with Docker Compose (requires env vars)
```

## Non-obvious conventions

- **Config binding**: `FaultLensSampleSettings` binds the `FaultLens` section. Environment variable override uses double-underscore: `FaultLens__ApiKey`, `FaultLens__Endpoint`, `FaultLens__Environment`, `FaultLens__Release`.
- **Missing ApiKey throws at startup** — `Program.cs` throws `InvalidOperationException` if `FaultLens:ApiKey` is absent. This is intentional sample behavior, not a production pattern.
- **`FaultLensClient` registered as both `FaultLensClient` and `IFaultLensClient`** — controllers inject `IFaultLensClient`; the singleton is wired in `Program.cs`.
- **`FaultLensExceptionHandler`** is the global unhandled exception capture path — preserve it unless the issue requires changes.
- **No tests exist** — `dotnet test` is a no-op unless tests are added by the issue.

## ClaudeCode-specific notes

- Stay implementation-first. Keep the sample simple — do not expand it into a product application.
- Read only the files needed for the task before editing. Prefer `Edit` (targeted diff) over full rewrites.
- Keep diffs narrow — no formatting churn, no unrelated renames.
- If sample behavior or run instructions change, update `README.md` in the same commit.
- After validation, update the GitHub issue using `C:\PersonalProjects\faultlens-ui\issue-body.md` and `gh issue comment`.
- Do not deploy or publish unless explicitly requested.
