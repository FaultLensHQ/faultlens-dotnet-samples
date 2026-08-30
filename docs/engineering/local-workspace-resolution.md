# Local workspace and repository resolution

FaultLens distinguishes a GitHub repository identity from the filesystem workspace used for execution.

## Local-first execution contract

When an execution agent is running in a developer workstation environment and a FaultLens repository already has a local checkout, the existing local checkout is the preferred workspace for code investigation, editing, builds, focused tests, migrations, and other repository-local execution.

An `owner/repository` identifier such as `FaultLensHQ/faultlens-backend` identifies the durable GitHub repository. It does not imply that the agent should clone, reconstruct, or use a connector-backed copy as its working tree.

Before cloning or reconstructing a repository, an execution agent must:

1. resolve the requested GitHub repository identity against the configured local workspace root or repository map;
2. verify that the candidate directory is the intended Git working tree, including its remote identity where practical;
3. reuse that checkout when it is suitable for the active work;
4. use GitHub for durable issues, pull requests, review state, hosted CI, remote refs, and other authoritative remote evidence rather than as a substitute working tree.

Do not clone merely because a prompt names a repository as `FaultLensHQ/<repo>`.

## FaultLens workstation mapping

For the current primary Windows development workstation, the configured FaultLens workspace root is:

```text
C:\PersonalProjects
```

The default mapping is repository-name based:

```text
FaultLensHQ/faultlens-backend     -> C:\PersonalProjects\faultlens-backend
FaultLensHQ/faultlens-ui          -> C:\PersonalProjects\faultlens-ui
FaultLensHQ/faultlens-admin-ui    -> C:\PersonalProjects\faultlens-admin-ui
FaultLensHQ/faultlens-marketing   -> C:\PersonalProjects\faultlens-marketing
FaultLensHQ/faultlens-sdk-js      -> C:\PersonalProjects\faultlens-sdk-js
FaultLensHQ/faultlens-sdk-dotnet  -> C:\PersonalProjects\faultlens-sdk-dotnet
FaultLensHQ/faultlens-engineering -> C:\PersonalProjects\faultlens-engineering
```

Treat this mapping as execution-environment configuration, not product or architecture truth. If a mapped directory does not exist or its Git remote does not correspond to the requested repository, stop and resolve the discrepancy rather than silently cloning into an arbitrary location or operating on the wrong checkout.

A different workstation or execution environment may provide a different workspace root or explicit repository map. The invariant is local-first resolution when a valid configured checkout exists, not the Windows path itself.

## Repository state safety

Finding the correct local checkout does not authorize destructive cleanup or branch mutation.

Before implementation, inspect relevant repository state and preserve unrelated work. Do not delete worktrees, reset changes, switch branches destructively, or overwrite another task's work merely to obtain a clean workspace. Use an appropriate existing clean worktree or create a bounded worktree when repository-local governance permits it.

Remote freshness still matters. Local-first means reuse the correct working copy; it does not mean trust stale local refs. Verify the required base/head/remote state for the active task before making correctness or review claims.

## Relationship to validation and PR governance

This contract does not change governance 1.2.8 validation or review-state policy:

- implementation PRs normally open non-Draft / Ready for review;
- execution agents do not self-approve or merge their own implementation;
- focused local validation proves the changed behavior and critical invariants;
- exact-head hosted PR CI is the authoritative repository-wide regression gate where available;
- broader/full local suites remain exception-driven under the existing validation contract.

The purpose of local-first resolution is to remove repeated repository discovery/cloning cost without weakening repository safety, validation, durable GitHub evidence, or independent review.
