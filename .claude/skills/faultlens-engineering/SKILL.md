---
name: faultlens-engineering
description: Route non-trivial FaultLens .NET sample work through current SDK/product authority, bounded implementation, focused validation, and strict independent Review.
---

# FaultLens Engineering Router — .NET samples

This is a thin router. Root `AGENTS.md` and `CLAUDE.md` remain the repository-local overlay for sample safety, SDK integration behavior, and validation.

## Operating-role boundary

Canonical role governance comes from `FaultLensHQ/faultlens-engineering/docs/agents/operating-roles.md`, governance **1.2.11**, released source commit `a32927ebd6b5670293d16ae0644624f11bff2757`.

- **AI Governor** applies to agent/skill/governance/rule/version/distribution changes.
- **Architecture & Product Governor** independently reviews returned implementation against the actual current PR/head/base/diff/evidence.
- **Execution Agent** may implement, remediate, test and update PR evidence, but does not approve or merge its own implementation.
- Sample work must not redefine SDK public API, capture/privacy semantics, package versions, ingestion contracts or product truth. Route those decisions to the owning SDK/backend work first.

## Compute and validation economy

Canonical compute/context governance comes from `FaultLensHQ/faultlens-engineering/docs/ai-compute-context-economy.md`, governance **1.2.11**, released source commit `a32927ebd6b5670293d16ae0644624f11bff2757`.

- Use strongest reasoning for unresolved product/public-contract/privacy/design questions and strict independent Review.
- Use a medium-cost capable execution model for normal implementation once the contract is settled, and the lowest-cost capable tier for deterministic/repetitive execution.
- Locally run build/compile and focused affected tests or Docker validation sufficient to prove the changed sample behavior.
- Do not routinely duplicate the complete repository regression matrix locally when hosted PR CI runs it on the exact pushed head.
- Hosted PR CI is the authoritative full-regression merge gate where available; broader local suites remain appropriate for uncertain blast radius, CI/build/test infrastructure changes, CI-failure reproduction, unavailable hosted CI, or repository-specific requirements.
- Never weaken, skip, quarantine, baseline or ignore failures to obtain green evidence.

## Workspace and repository resolution

Canonical local-workspace governance comes from `FaultLensHQ/faultlens-engineering/docs/engineering/local-workspace-resolution.md`, governance **1.2.11**, released source commit `a32927ebd6b5670293d16ae0644624f11bff2757`.

Before cloning or reconstructing a FaultLens repository, resolve `FaultLensHQ/<repo>` against the configured local workspace/repository map, verify the candidate Git working tree and remote identity, and reuse the valid local checkout. GitHub remains authoritative for durable issues, PRs, review state, hosted CI and remote evidence. Local-first execution does not authorize stale-ref assumptions or destructive worktree cleanup.

## Pull-request validation and review state

Canonical PR validation/review governance comes from `FaultLensHQ/faultlens-engineering/docs/engineering/pr-validation-and-review-state.md`, governance **1.2.11**, released source commit `a32927ebd6b5670293d16ae0644624f11bff2757`.

- Open implementation PRs non-Draft / Ready for review by default; GitHub Ready is not merge approval.
- While incomplete, record `IMPLEMENTATION IN PROGRESS — NOT READY FOR GOVERNOR REVIEW`.
- When the final pushed head and focused proof are ready, record `READY FOR INDEPENDENT REVIEW`.
- Only the independent governor may establish `APPROVED / MERGE-READY`.
- Any commit after governor PASS invalidates approval for the previous head.
- Independent review may proceed while CI runs; merge remains blocked until required exact-head hosted gates are green.

Read root `AGENTS.md`, `CLAUDE.md`, the tracked issue, current solution/project/runtime configuration, and the smallest affected sample path. Do not publish packages or deploy unless explicitly authorized.
