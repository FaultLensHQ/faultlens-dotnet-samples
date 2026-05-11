# AGENTS.md — faultlens-dotnet-samples

Canonical shared instruction file for all coding agents (ClaudeCode, Codex, and equivalents).
This repo owns **.NET sample applications** for FaultLens — minimal, runnable apps that help developers validate and demonstrate FaultLens .NET SDK integration before wiring it into their own applications.
Read this before starting any task in this repo.

---

## Sample principle

- Samples should help developers understand and validate FaultLens .NET SDK integration quickly.
- Samples should demonstrate useful error, event, message, and breadcrumb capture flows.
- Keep samples simple and realistic — they are onboarding tools, not product applications.
- Do not turn samples into full product apps with auth, dashboards, or admin features.
- Every sample change should support faster onboarding, SDK validation, or triage-flow demonstration.
- SDK failures in samples should be visible enough for validation but must not teach unsafe production patterns.

---

## Work mode

- Aggressive build mode. Implementation-first.
- Minimal, production-safe changes.
- Avoid broad rewrites unless explicitly required.
- Keep sample flows easy to run locally.

---

## GitHub tracking workflow

- Open a GitHub issue before starting feature work. Do not create duplicate issues.
- Use `C:\PersonalProjects\faultlens-ui\issue-body.md` as the scratch file for issue bodies and comments.
- After validation, update the issue using `gh issue comment` with `--body-file`.
- Do not close issues unless implementation is complete and validated.
- Keep GitHub CLI commands simple.

---

## Repo, branch, and release rules

- Repo name: `faultlens-dotnet-samples`.
- Follow the existing branch convention used by this repo. Do not switch branch strategy casually.
- Do not publish NuGet packages — samples are not packaged.
- Do not deploy unless explicitly requested.

---

## Sample app rules

- **No hardcoded secrets**: configuration must come from environment variables, `appsettings.Development.json` (gitignored values), or Docker environment variables. Do not commit real API keys, tenant hosts, connection strings, or private endpoints.
- **Configuration variables**: the expected config keys are `FaultLens__ApiKey`, `FaultLens__Endpoint`, `FaultLens__Environment`, and `FaultLens__Release`. Use these names consistently.
- **SDK version**: the sample uses `FaultLens.Sdk` v0.1.0-beta.1. Do not bump the SDK version unless the issue explicitly requires it.
- **Target framework**: `net10.0`. Do not change this unless the issue explicitly requires it.
- **Preserve sample flows**: DI registration, request-scoped breadcrumbs (`BeginRequest`), manual breadcrumbs (`AddStep`, `AddDecision`), message capture, handled exception capture, and global unhandled exception capture. Do not remove or quietly change these unless the issue requires it.
- **Intentional sample errors**: exceptions and errors in sample code should be deliberate, clearly labelled, and useful for validating FaultLens capture behavior. Do not add accidental or silent failures.
- **Docker**: preserve the Docker and docker-compose local run flow if present.
- **Keep README aligned**: if sample behavior, configuration, or run instructions change, update `README.md` to match.
- **Avoid unnecessary dependencies**: keep the sample dependency footprint minimal.

---

## Validation expectations

- Run `dotnet build faultlens-dotnet-samples.slnx` to confirm the solution builds cleanly.
- Run `dotnet test faultlens-dotnet-samples.slnx` if tests exist or were added.
- If Docker files or sample run docs change, validate or clearly state what was not locally validated.
- Include exact commands and results in the final response.

---

## Final response format

Max 8 bullets covering:

- Files changed
- What changed and why
- Validation commands and results
- GitHub issue update status
- Follow-up notes (only when useful)
