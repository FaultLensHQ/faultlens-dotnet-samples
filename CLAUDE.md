# CLAUDE.md — faultlens-dotnet-samples

Read `AGENTS.md` first. This file contains Claude-specific sample execution notes only.

## Purpose and context

FaultLens .NET samples are minimal runnable integration/onboarding applications. They demonstrate the currently supported .NET SDK contract; they do not define that contract.

Before editing:

- read the affected sample, current solution/project files and the SDK public API it uses;
- derive current SDK version, target framework, language/build settings and configuration keys from authoritative project/sample configuration rather than this file;
- route any required SDK public-contract, privacy, capture, or ingestion semantic change to the owning SDK/backend Product Decision/Design work.

## Stable sample conventions

- Never commit real API keys, tenant hosts, credentials, connection strings or private endpoints.
- Preserve the repository's supported environment/appsettings/Docker configuration path and fail-closed sample configuration behavior where currently intentional; read exact keys from current code/configuration.
- Preserve the sample's DI/request-scope, breadcrumb/message, handled/unhandled exception and global capture demonstrations unless the tracked issue intentionally changes the teaching flow.
- Keep intentional sample failures clearly labelled; do not teach accidental or silent failure patterns.
- Keep Docker/docker-compose and README/run instructions aligned with affected behavior.
- Keep the sample small; do not expand it into a FaultLens product application.

## Editing and validation

- Prefer targeted diffs and avoid unrelated formatting/renames.
- Use current solution/project configuration for build/tests and Docker validation where applicable; report exact commands/results.
- Persist material decisions/evidence in GitHub using repository-neutral temporary files when a body file is useful.
- Do not deploy, publish packages, or change SDK/product semantics unless explicitly authorized through the owning work.
