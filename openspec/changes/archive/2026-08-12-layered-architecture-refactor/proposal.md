## Why

Commit `97700f1` converted the solution to an N-Layer (layered) architecture — four projects under `src/` (Api, BusinessLogic, DataAccess, Entities) — but the structure is not codified as a contract and the repository carries leftovers from the previous flat layout. This change documents the layered architecture as a verifiable capability spec and audits/verifies that the tree conforms — with no behavior change, no namespace renames, and no modification of the read-only integration tests (same convention as the previously archived vertical-slice and clean-architecture changes).

## What Changes

- Add a `layered-architecture` capability spec establishing the structural contract: four projects under `src/` (`HackerRank1.Api`, `HackerRank1.BusinessLogic`, `HackerRank1.DataAccess`, `HackerRank1.Entities`) with layer responsibilities and a one-directional dependency rule.
- Audit the existing tree against the contract:
  - Verify project references are exactly `Api → BusinessLogic → DataAccess → Entities` and `Entities` references nothing.
  - Verify each `.cs` file sits in its layer's folder (`src/<Project>/<Layer-Folder>/...`).
- Verify namespace preservation: `LibraryService.WebAPI.*` and `HackerRank1.*` namespaces stay exactly as-is (the read-only `LibraryService.Integration.Test` depends on them).
- Remove stray leftovers of the previous flat layout if any remain untracked (verified before deleting).
- `LibraryService.Integration.Test/IntegrationTest.cs` and `IntegrationTest/` stay untouched (read-only / legacy out-of-scope).

## Capabilities

### New Capabilities
- `layered-architecture`: structural contract for the N-Layer layout — layer responsibilities, one-way dependency direction, preserved legacy namespaces, read-only integration tests, and a repository free of stray layout leftovers.

### Modified Capabilities
<!-- None: `openspec/specs/` is empty on this branch and no runtime behavior changes. -->

## Impact

- **Code**: `src/` project structure and `.csproj` reference graph (audit/verification only; no source moves or content edits expected).
- **APIs**: none — endpoints, routes, models, and namespaces unchanged.
- **Tests**: `LibraryService.Integration.Test` (read-only) and the legacy `IntegrationTest/` project are not modified; the solution must still build and the existing csproj fix (orphaned MSTest `Using`) stays as committed.
- **Repository**: possible deletion of untracked leftover folders from the earlier flat layout; no dependency changes.