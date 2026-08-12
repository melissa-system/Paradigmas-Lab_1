## Context and Problem Statement

Commit `97700f1` ("Convertir HackerRank1 a arquitectura N-Layer") already restructured the solution into four projects (`src/HackerRank1.Api`, `src/HackerRank1.BusinessLogic`, `src/HackerRank1.DataAccess`, `src/HackerRank1.Entities`), and commit `ca46a19` fixed the orphaned MSTest `Using` in the test csproj. What remains is (1) anchoring that structure as an explicit, verifiable contract, and (2) verifying the tree actually conforms — references, folder placement, namespaces — while keeping the read-only `LibraryService.Integration.Test` compiling unchanged against the legacy `LibraryService.WebAPI.*` and `HackerRank1.*` namespaces.

## Goals and Non-Goals

**Goals**
- Codify the N-Layer layout as the `layered-architecture` capability.
- Verify project references match exactly `Api → BusinessLogic → DataAccess → Entities`, with Entities referencing nothing.
- Verify each production `.cs` file lives in its layer's folder under `src/<Project>/<Layer>/`.
- Verify no namespace/type renames happened since `97700f1` (diff `d732b15..HEAD` for `src/`).
- Remove verified-dead leftover folders from the previous flat layout, if any.

**Non-Goals**
- No behavior changes: endpoints, routes, logic, `LibrariesService.Delete` (NotImplementedException) all stay as-is.
- No namespace or type renames (`LibraryService.WebAPI.*`, `HackerRank1.*` preserved).
- No modification of `LibraryService.Integration.Test/IntegrationTest.cs`, `IntegrationTest/IntegrationTest.cs`, or the legacy `IntegrationTest/` project — read-only by convention.
- No dependency upgrades, no package changes, no schema changes.
- Do NOT attempt to fix the pre-existing EF Core 6-vs-8 test runtime conflict (`MissingMethodException`); out-of-scope, same as in the archived vertical-slice/clean-architecture changes.

## Solution

### 1. Ground-truth audit (no edits yet)
- Enumerate solution projects (`dotnet sln list`) → expect exactly Api, BusinessLogic, DataAccess, Entities + LibraryService.Integration.Test.
- Read all four csproj files → tabulate `<ProjectReference>` entries.
- List all tracked/untracked files under `src/` and at repo root → identify strays (e.g. leftover `HackerRank1/` or `LibraryService.WebAPI/` folders with zero tracked content).
- `git diff d732b15..HEAD -- src/` → confirm the N-Layer conversion is the only structural change (adds project folders, no renames).

### 2. Verify namespace preservation
- Grep namespace declarations across `src/**/*.cs` → confirm the set matches the one that predates this change (`LibraryService.WebAPI.*`, `LibraryService.WebAPI.Data`, `LibraryService.WebAPI.DTO`, `HackerRank1.*`).
- Confirm `LibraryService.Integration.Test/IntegrationTest.cs` is byte-identical to its state on `d732b15` (git diff against that commit for the test file).

### 3. Remove stray leftovers (only if verified dead)
- For each untracked stray folder: confirm zero tracked files inside (`git ls-files <folder>` empty) before deleting; then `Remove-Item`; document each removal in the apply report.

### 4. Conformance check
- Rebuild the solution (`dotnet build HackerRank1.sln`) → must compile (it already does after `ca46a19`; re-verify).
- Optionally `dotnet test` (does NOT require the EF runtime Mvc.Testing stack? — test run invokes it; baseline: tests either pass or fail with the known out-of-scope `MissingMethodException`; either outcome is recorded, no fixing).

### 5. Also inspect (leftovers cleanup opportunity)
- `openspec/` on this branch is empty/unversioned; do not commit planning artifacts here — this change is doc+audit only. (If the user later wants the OpenSpec backlog committed, that is a separate call.)

## Risky and Hard Parts

- **Read-only test coupling**: `IntegrationTest.cs` pins legacy namespaces; any accidental namespace change breaks the build of the test project. Guarded by step 2's byte-identical check.
- **Stray-folder deletion**: only delete untracked, file-free folders; never touch `src/` folders that contain tracked code. Guarded by `git ls-files` emptiness check before every deletion.
- **EF Core 6-vs-8 test conflict**: known runtime `MissingMethodException` in the integration tests; re-appearing during verification is expected and explicitly out of scope.

## Alternative Solutions Considered

- **Re-architect into drop-in-namespace projects** (rename namespaces to match folders): rejected — breaks the read-only test's `LibraryService.WebAPI.*` imports; violates the "no renames" convention established in the two archived changes.
- **Fold Document-style projects back into one**: rejected — `97700f1` conversion is already committed and this change exists to codify it, not undo it.