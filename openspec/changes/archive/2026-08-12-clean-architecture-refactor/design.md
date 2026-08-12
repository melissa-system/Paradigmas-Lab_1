## Context

See proposal.md — Why for motivation. Relevant current state:

- Commit `b565921` already converted the solution to Clean Architecture: `src/HackerRank1.Domain`, `src/HackerRank1.Application`, `src/HackerRank1.Infrastructure`, `src/HackerRank1.Api` (all net8.0), verified references `Api → Infrastructure → Application → Domain` with Domain referencing nothing. The legacy flat `HackerRank1/` project was removed from git; only an untracked leftover folder with empty directories and build output remains at the root.
- Namespaces were deliberately preserved (`LibraryService.WebAPI.*`, `HackerRank1.*`) so the read-only `LibraryService.Integration.Test/IntegrationTest.cs` keeps compiling; its csproj project reference was repointed from `..\HackerRank1\HackerRank1.csproj` to `..\src\HackerRank1.Api\HackerRank1.Api.csproj`.
- The test project packages are `Microsoft.AspNetCore.Mvc.Testing 6.0.0`, `EntityFrameworkCore.InMemory/Sqlite 6.0.0`, while the API pulls EF Core 8.0.2 via Npgsql — a known pre-existing version conflict that surfaced on the sibling branch and is expected to reproduce here (runtime, not compile-time; out of scope).
- `openspec/changes/` on this branch is a fresh planning root; `openspec/specs/` is empty, so this change's `clean-architecture` delta uses only ADDED requirements.

## Goals / Non-Goals

**Goals:**
- Establish the Clean Architecture layout as a verifiable spec contract.
- Verify (not redo) the conversion of `b565921`: project set, one-directional references, layer responsibilities, namespace preservation.
- Remove the untracked leftover root `HackerRank1/` folder.
- Prove behavior-neutrality: solution builds, tests compile; zero edits to production source or tests.

**Non-Goals:**
- No namespace/type renames, no source moves, no content edits under `src/`. Namespace/folder mismatch is expected (legacy namespaces by design) and is not a violation.
- No behavior changes and no bug fixes (e.g. `LibrariesService.Delete` keeps its `NotImplementedException`).
- No modification of `LibraryService.Integration.Test/IntegrationTest.cs` or the legacy `IntegrationTest/` project; the only allowed exception (user-approved, see Change Notes) is removing the orphaned `<Using Include="Microsoft.VisualStudio.TestTools.UnitTesting" />` line from `LibraryService.Integration.Test.csproj` that carries a pre-existing `CS0234`.
- Fixing the pre-existing EF Core 6-vs-8 test-version conflict is out of scope; it is documented, not resolved.
- No CI enforcement (future work).

## Decisions

**D1 — Verification-first audit, not speculative moves**
The `b565921` conversion is the baseline. Implementation is a checklist-driven audit: enumerate all `.cs` files under `src/`, assert layer membership per project (Domain: entities only; Application: contracts/DTOs/settings; Infrastructure: persistence/services/auth/token generator; Api: controllers/hosting), and assert the `<ProjectReference>` graph is exactly the chain. Move files only if the audit finds a violation (expected: none).
Rationale: avoids churn and conflicts on the just-converted layout.
Alternative considered: re-executing the move mechanically — rejected as redundant.

**D2 — Zero-touch rule for code, csproj, and tests**
No `.cs` or `.csproj` file under `src/` is edited; the two integration test files and the legacy test project are never touched. Verification is by diff: `git diff` on `src/`, `LibraryService.Integration.Test/`, `IntegrationTest/` must be empty at the end. Namespace preservation is guaranteed structurally.
Alternative considered: namespace aliases/type-forwarders — rejected; adds moving parts and hides the real constraint.

**D3 — Cleanup of the leftover root `HackerRank1/` folder via plain deletion**
Verified: `git ls-files` tracks nothing under the root `HackerRank1/` folder (empty dirs + `bin`/`obj` output only). Removing it is a plain filesystem delete with no git operation. `src/*/bin` and `src/*/obj` (live build output) stay.
Alternative considered: `git rm` or gitignore edits — unnecessary; the folder is untracked.

**D4 — Acceptance is build + test-compile, unchanged**
`dotnet build HackerRank1.sln` must succeed. `dotnet test` on `LibraryService.Integration.Test` is attempted; based on the sibling-branch experience it is expected to fail at runtime with `MissingMethodException` (EF Core version conflict, pre-existing) — the result is recorded as evidence in the change notes either way, and is not treated as caused by or fixable within this change.
Rationale: mirrors the documented constraint set; keeps the read-only test untouched.

## Risks / Trade-offs

- Audit criteria misapplied (namespace/folder mismatch treated as violation) → Mitigation: D1 checklist fixed before starting; legacy namespace usings are explicitly non-violations.
- Deleting root `HackerRank1/` surprises an IDE that still has it open → Mitigation: verify untracked status first (done in design), then delete; subsequent `dotnet build` confirms nothing references it.
- Expected EF Core 6-vs-8 runtime test failure blamed on this change → Mitigation: pre-existing condition documented in change notes; evidence (build success, compile success, exact exception) recorded.
- Future drift of the dependency graph → Mitigation: archived capability spec documents the contract; CI enforcement is a non-goal.

## Migration Plan

1. Audit step (D1) — no production changes expected.
2. Delete leftover root `HackerRank1/` folder (D3).
3. `dotnet build` + attempted `dotnet test` (D4); rerun `git status` to confirm no tracked changes outside planning artifacts.
4. Rollback: the only deletion is untracked build leftovers; source state is untouched, so rollback is restoring the folder or rebuilding.

## Change Notes (apply-time audit results)

Recorded while applying this change (2026-08-12). Evidence for the spec scenarios.

### 1.1 File placement inventory (22 tracked production `.cs` files under `src/`, excluding `obj/`)
- `src/HackerRank1.Domain/Entities/`: `Book.cs`, `Library.cs` (pure entities, no EF attributes — only comments referencing the `[Key]` convention)
- `src/HackerRank1.Application/`: `Auth/Dtos/User.cs`, `Auth/IAuthenticationService.cs`, `Books/Dtos/BookForm.cs`, `Books/IBooksService.cs`, `Libraries/Dtos/LibraryForm.cs`, `Libraries/ILibrariesService.cs`, `Common/Settings/JwtSettings.cs`
- `src/HackerRank1.Infrastructure/`: `Auth/TokenGenerator.cs`, `Persistence/LibraryContext.cs`, `Persistence/Migrations/` (3 migration files), `Services/AuthenticationService.cs`, `Services/BookService.cs`, `Services/LibraryService.cs`
- `src/HackerRank1.Api/`: `Controllers/AuthController.cs`, `Controllers/BooksController.cs`, `Controllers/LibrariesController.cs`, `Program.cs`, `Startup.cs` (plus `appsettings*.json`, `Properties/launchSettings.json`)

### 1.2-1.5 Assertions
- Solution contains exactly the four `src/` production projects + `LibraryService.Integration.Test`; no legacy flat project reference.
- Reference graph (from csproj `<ProjectReference>`): `Domain → []`, `Application → [Domain]`, `Infrastructure → [Application]`, `Api → [Infrastructure]` — exactly the `Api → Infrastructure → Application → Domain` chain; no sideways or reverse edges.
- Layer responsibilities hold: Domain entities carry no EF attributes; Application contains only contracts/DTOs/settings; Infrastructure holds persistence (`Persistence/` + migrations), service implementations (`Services/`), and `Auth/TokenGenerator.cs`; Api holds `Controllers/` + hosting files only.
- Api consumes services through Infrastructure-referenced interfaces (registered in `Startup.cs` via DI against the preserved namespaces); no direct `ProjectReference` from Api to Domain or Application.
- No violations found — task 1.6 not triggered; no file moves or reference changes required.

### 2.1-2.3 Cleanup
- `git ls-files` shows nothing tracked under the root `HackerRank1/` folder (leftover: empty dirs + `bin`/`obj` only).
- Deleted the root-level `HackerRank1/` folder.
- `src/*/bin` and `src/*/obj` left untouched.

### 3.1-3.2 Zero-touch results
- `git diff --stat` for `src/`, `LibraryService.Integration.Test/`, `IntegrationTest/`: empty — zero tracked source changes.
- `git diff b565921 -- src`: empty — zero namespace declarations or public type names renamed.
- `LibraryService.Integration.Test/IntegrationTest.cs` and `IntegrationTest/IntegrationTest.cs`: untouched (also confirmed by diff).

### 4.1-4.3 Build/test proof
- **Scope note (user-approved expansion):** the initial solution build failed with pre-existing `CS0234` — `LibraryService.Integration.Test.csproj` declared `<Using Include="Microsoft.VisualStudio.TestTools.UnitTesting" />` (line 29) but the project uses xunit; that namespace was previously supplied transitively by `MSTest.TestFramework 3.8.2`, which commit `b565921` dropped when the old flat `HackerRank1.csproj` became `src/HackerRank1.Api`. Verified pre-existing at the branch tip: `git diff b565921 -- src LibraryService.Integration.Test` was empty before any edit. Per user decision the orphaned `<Using>` line was removed from the test csproj (the only file edit of this change; test `.cs` files untouched).
- `dotnet build HackerRank1.sln` after the fix: succeeded — 0 errors, 6 warnings (all pre-existing/transitive, none introduced by the change).
- `dotnet test LibraryService.Integration.Test/LibraryService.Integration.Test.csproj`: compiles and runs, all 3 tests fail at runtime with `System.MissingMethodException: ConventionSet.get_ModelFinalizingConventions()` — the **pre-existing EF Core 6-vs-8 version conflict** (test packages `Mvc.Testing`/`InMemory`/`Sqlite` 6.0.0 vs API EF Core 8.0.2 via Npgsql), identical to the sibling-branch behavior and expected per design D4; out of scope, documented not fixed. No test file was modified.

### 5.1 Spec-contract location
- `openspec/specs/clean-architecture/spec.md` does not exist yet at apply time; it is created during archive-time sync from the delta at `openspec/changes/clean-architecture-refactor/specs/clean-architecture/spec.md`.