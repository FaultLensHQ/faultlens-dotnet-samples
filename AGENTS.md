# AGENTS.md — faultlens-dotnet-samples

Repository-local overlay for FaultLens .NET sample applications.

Samples are onboarding/integration tools, not product applications or SDK authority.

## Purpose

- demonstrate FaultLens .NET SDK integration quickly and realistically;
- show intentional error/message/breadcrumb/request-scope capture flows;
- remain minimal, runnable and safe to learn from.

## Stable rules

- Never commit real API keys, tenant hosts, connection strings, credentials or private endpoints.
- Use the repository's supported environment/appsettings/Docker configuration mechanism.
- Read current SDK/package versions, target frameworks and build settings from project files/solution configuration. Do not duplicate volatile values in evergreen governance.
- Preserve the owning SDK's public contract; samples must not invent a new capture/ingestion model.
- Keep intentional sample failures clearly labelled and useful for validation.
- Preserve existing DI/request-scope/breadcrumb/message/handled/unhandled demonstration flows unless the tracked issue intentionally changes them.
- Keep README/run instructions aligned when behavior/configuration changes.
- Avoid unnecessary dependencies.

## Decision boundary

A sample task must not redefine SDK public API, capture/privacy semantics, package versions or ingestion contracts. Route such work to the owning SDK/backend Product Decision/Design first.

**Discovery does not imply priority.** Sample cleanup does not automatically become SDK/product redesign.

## Repository discipline

- GitHub issues/PRs are the durable work record.
- Persist decisions/evidence in GitHub rather than workstation-specific scratch paths.
- Follow this repo's actual default branch/configuration.
- Do not publish packages or deploy unless explicitly authorized.

## Validation

Use the current solution/project configuration for build/tests and Docker validation where applicable. Report exact commands/results and unvalidated surfaces; never claim an unexecuted check passed.
