## Context

See proposal.md — Why for motivation. Relevant current state:

- Commit `bc71eda` already moved folder layout into `Features/` + `Common/` (controller split into partial classes, one file per use-case). Namespaces were deliberately left untouched so the read-only `LibraryService.Integration.Test/IntegrationTest.cs` keeps compiling.
- `openspec/specs/` is empty; this change introduces the first capability, so the delta spec for `vertical-slice-architecture` uses only ADDED requirements.
- The solution (`HackerRank1.sln`) contains exactly two projects: `HackerRank1` (the API) and `LibraryService.Integration.Test`. The `IntegrationTest/` folder is a legacy project not in the solution.
- `bin/` and `obj/` are gitignored; `src/HackerRank1.Api/` (only `obj/` output) and root-level `bin/`, `obj/` are untracked build output, not repository content.

## Goals / Non-Goals

**Goals:**
- Establish the vertical-slice structure as a verifiable spec contract.
- Verify (not redo) the folder organization: every production `.cs` in `HackerRank1/` sits under `Features/` or `Common/`; no legacy layer folders remain.
- Remove stray build-output folders (`src/HackerRank1.Api/`, root `bin/`, `obj/`) from the working tree.
- Prove the refactor is behavior-neutral: solution builds and integration tests pass with zero edits to production code or tests.

**Non-Goals:**
- No namespace or type renames, no type moves, no content edits to `HackerRank1/` source files. Namespace/folder mismatch is expected (namespaces are legacy by design) and is not a violation.
- No behavior changes and no bug fixes (e.g. `LibrariesService.Delete` still throws `NotImplementedException`).
- No modification of `LibraryService.Integration.Test/IntegrationTest.cs`, the `IntegrationTest/` project, or `.gitignore`.
- No CI automation to enforce the structure (future work).

## Decisions

**D1 — Verification-first audit, not speculative moves**
The folder conversion from `bc71eda` is the baseline. Implementation is a checklist-driven audit: enumerate all production `.cs` files, assert each is under `Features/` or `Common/`, assert no legacy folder names (`Controllers/`, `DTO/`, `Entities/`, `Helpers/`, `Services/`, `Data/`) exist under the project. Move files only if the audit finds a violation (expected: none).
Rationale: avoids churn, empty commits, and conflicts on folders that were just reorganized.
Alternative considered: re-executing the move mechanically — rejected as redundant and risky.

**D2 — Zero-touch rule for code and tests**
No `.cs` file under `HackerRank1/` is edited; the two integration test files are never touched. Verification is by diff: `git diff` on the production and test trees must be empty at the end of each step. Namespace preservation is therefore guaranteed structurally, and the read-only constraint is trivially satisfied.
Alternative considered: adding `global using` aliases or type-forwarding so future namespace renames wouldn't break the tests — rejected; it adds moving parts and hides the real constraint, which is "namespaces stay".

**D3 — Cleanup of stray build output via plain deletion**
`src/HackerRank1.Api/`, root `bin/`, and root `obj/` contain only ignored/untracked build output (verified: nothing under `src/`, `bin/`, `obj/` is tracked by git). Removing them is a local filesystem delete with no git operation. `HackerRank1/obj` is left alone (live project output).
Alternative considered: `git rm` or gitignore additions — unnecessary since the folders are untracked and already ignored.

**D4 — Acceptance is build + integration tests, unchanged**
`dotnet build HackerRank1.sln` and `dotnet test` on `LibraryService.Integration.Test` must pass without any modification. This is the observable proof for the "read-only test keeps compiling" and "no behavior change" requirements; no new tests are written.

## Risks / Trade-offs

- Audit criteria misapplied (treating namespace/folder mismatch as a violation, where the whole point is preserving legacy namespaces) → Mitigation: D1 checklist is fixed before starting; namespace usings referencing folder-less namespaces are explicitly non-violations.
- Deletion of `src/HackerRank1.Api/` surprises an IDE/folder-watcher even though it is not in the solution → Mitigation: delete only object-output-only folders; confirm with `dotnet build` afterwards.
- A future developer reintroduces layer-style folders (drift after archive) → Mitigation: the archived capability spec documents the contract; CI enforcement is explicitly a non-goal of this change.
- Spec states structural facts (folder placement) that are only enforced by convention → Mitigation: each requirement maps to a scenario checkable by a git/filesystem listing, so review is objective.

## Migration Plan

1. Audit step (D1) — no production changes expected.
2. Delete `src/HackerRank1.Api/`, root `bin/`, root `obj/` (D3).
3. `dotnet build` + `dotnet test` (D4); rerun `git status` to confirm no tracked-file changes outside `openspec/`.
4. Rollback is trivial: all deletions are ignored build output, so regenerating via build restores the prior working-tree state; no source tracked content changes exist to revert.

## Change Notes (apply-time audit results)

Recorded while applying this change (2026-08-12). Evidence for the spec scenarios; user-approved exceptions noted explicitly.

### 1.1 File placement inventory (22 production `.cs` files under `HackerRank1/`, excluding `obj/`)
- `HackerRank1/Common/Persistence/`: `LibraryContext.cs`; `Migrations/20260528004745_InitialCreate.cs`, `20260528004745_InitialCreate.Designer.cs`, `LibraryContextModelSnapshot.cs`
- `HackerRank1/Common/Settings/`: `JwtSettings.cs`
- `HackerRank1/Features/Auth/Login/`: `AuthController.cs`, `AuthenticationService.cs`, `TokenGenerator.cs`, `TokenResponse.cs`, `User.cs`
- `HackerRank1/Features/Books/GetBooksByLibrary/`: `BooksController.cs`, `BookService.cs`, `BookForm.cs`
- `HackerRank1/Features/Libraries/CreateLibrary/`: `LibrariesController.Add.cs`, `LibraryForm.cs`
- `HackerRank1/Features/Libraries/DeleteLibrary/`: `LibrariesController.Delete.cs`
- `HackerRank1/Features/Libraries/GetLibraries/`: `LibrariesController.GetAll.cs`
- `HackerRank1/Features/Libraries/GetLibraryById/`: `LibrariesController.Get.cs`
- `HackerRank1/Features/Libraries/UpdateLibrary/`: `LibrariesController.Update.cs`
- `HackerRank1/Features/Libraries/Shared/`: `LibraryService.cs`
- `HackerRank1/` root: `Program.cs`, `Startup.cs` — **approved exception** (user decision): ASP.NET Core bootstrap/composition root stays at project root; the spec's "every file under Features/ or Common/" scenario is satisfied by all remaining 20 files.

### 1.2-1.5 Assertions
- 20 of 22 files are under `Features/<Feature>/<UseCase>/`, `Features/<Feature>/Shared/`, or `Common/` (exception above).
- No legacy layer folders (`Controllers/`, `DTO/`, `Entities/`, `Helpers/`, `Services/`, `Data/`) exist anywhere under `HackerRank1/` (directory scan, excluding `obj/`).
- Use-case folders are self-contained: each contains only its controller actions, request/response types, and service logic; `Features/Libraries/Shared/LibraryService.cs` is the only shared code and is referenced by all five library CRUD slices.
- `Common/` holds only cross-cutting code: `Persistence/` (DbContext + migrations) and `Settings/` (JwtSettings); no feature logic.
- No file moves were required (1.6 not triggered).

### 3.1-3.2 Zero-touch results
- `git diff --stat` vs. HEAD (`bc71eda`): empty for `HackerRank1/`, `LibraryService.Integration.Test/`, `IntegrationTest/` — zero tracked source changes.
- `git diff bc71eda -- HackerRank1`: empty — zero namespace declarations or public type names renamed.
- `LibraryService.Integration.Test/IntegrationTest.cs` and `IntegrationTest/IntegrationTest.cs`: untouched (also confirmed by diff).**

### 4.1-4.3 Build/test proof
- `dotnet build HackerRank1.sln`: succeeded — 0 errors, 24 warnings (all from untouched code; zero diffs vs. HEAD mean none are new). The read-only test file compiles unchanged.
- `dotnet test LibraryService.Integration.Test/LibraryService.Integration.Test.csproj`: compiles, but all 3 tests fail at runtime with `System.MissingMethodException: ConventionSet.get_ModelFinalizingConventions()` — **pre-existing, out-of-scope blocker** (user decision, recorded here): the test project references EF Core 6.0.0 (`Mvc.Testing`/`InMemory`/`Sqlite`) while the API references EF Core 8.0.2 via `Npgsql.EntityFrameworkCore.PostgreSQL`, and the loaded-api-vs-test-conventions mismatch reproduces identically at HEAD (`bc71eda`). The working tree is byte-identical to HEAD (this change deleted only ignored build output), so this change neither caused nor can fix the failure; fixing it would modify the test project's dependencies, which this change excludes. Task 4.2 remains blocked by this decision; no test file was modified.

### 5.1 Spec-contract location
- `openspec/specs/vertical-slice-architecture/spec.md` does not exist yet at apply time; it is created during archive-time sync from the delta at `openspec/changes/vertical-slice-refactor/specs/vertical-slice-architecture/spec.md`.