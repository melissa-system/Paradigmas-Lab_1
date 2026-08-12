## Why

Commit `b565921` converted the solution to Clean Architecture (four projects under `src/`: Domain, Application, Infrastructure, Api), but the structure is not codified as a contract and the repository still carries leftovers from the previous layout. This change documents the clean-architecture structure as a verifiable capability spec and audits/verifies that the tree conforms — with no behavior change, no namespace renames, and no modification of the read-only integration tests.

## What Changes

- Add a `clean-architecture` capability spec establishing the structural contract: four projects under `src/` (`HackerRank1.Domain`, `HackerRank1.Application`, `HackerRank1.Infrastructure`, `HackerRank1.Api`) with layer responsibilities (pure Domain entities; Application contracts + DTOs + cross-cutting settings; Infrastructure persistence, service implementations, and token generation; Api controllers and hosting) and a one-directional dependency rule.
- Audit the existing tree against the contract:
  - Verify project references are exactly `Api → Infrastructure → Application → Domain` and `Domain` references nothing.
  - Verify each `.cs` file sits in its layer's folder (`src/<Project>/<Layer-Folder>/...`).
- Verify namespace preservation: `LibraryService.WebAPI.*` and `HackerRank1.*` namespaces stay exactly as-is (the read-only `LibraryService.Integration.Test` depends on them).
- Remove the stray root-level `HackerRank1/` folder (leftover of the old flat project; contains no tracked files — only empty directories and build output).
- `LibraryService.Integration.Test/IntegrationTest.cs` and `IntegrationTest/` stay untouched (read-only / legacy out-of-scope).

## Capabilities

### New Capabilities
- `clean-architecture`: structural contract for the multi-project Clean Architecture layout — layer responsibilities, one-way dependency direction, preserved legacy namespaces, read-only integration tests, and a repository free of stray layout leftovers.

### Modified Capabilities
<!-- None: `openspec/specs/` is empty on this branch and no runtime behavior changes. -->

## Impact

- **Code**: `src/` project structure and `.csproj` reference graph (audit/verification only; no source moves or content edits expected).
- **APIs**: none — endpoints, routes, models, and namespaces unchanged.
- **Tests**: `LibraryService.Integration.Test/IntegrationTest.cs` (read-only) and the legacy `IntegrationTest/` project are not modified; the test project's `.csproj` receives one user-approved edit (removal of an orphaned MSTest `<Using>` that breaks the solution build with a pre-existing `CS0234`).
- **Repository**: deletion of the untracked leftover root `HackerRank1/` folder; no dependency changes.