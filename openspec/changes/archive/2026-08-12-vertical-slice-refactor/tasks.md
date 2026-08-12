## 1. Structural audit (verification-first, per design D1)

- [x] 1.1 Enumerate every production `.cs` file under `HackerRank1/` (excluding `obj/`) and record, in the change notes, the folder each file sits under
- [x] 1.2 Assert each production file is under `Features/<Feature>/<UseCase>/`, `Features/<Feature>/Shared/`, or `Common/`; report any file that is not
- [x] 1.3 Assert no legacy layer folders exist anywhere in `HackerRank1/`: `Controllers/`, `DTO/`, `Entities/`, `Helpers/`, `Services/`, `Data/`
- [x] 1.4 Assert each use-case folder contains only its own artifacts (controller action file, request/response types, service logic) and shared multi-use-case code lives under `Features/<Feature>/Shared/` (e.g. `Libraries/Shared/LibraryService.cs`)
- [x] 1.5 Assert `Common/` holds only cross-cutting code (`Persistence/`, `Settings/`, `Migrations/`) with no feature logic
- [x] 1.6 If the audit finds a violation, move the offending file to its correct slice folder without changing its namespace; confirm `git status` afterwards shows only that move

## 2. Cleanup of stray build artifacts (per design D3)

- [x] 2.1 Verify `git ls-files` shows nothing tracked under `src/`, `bin/`, or `obj/` (untracked-only before deleting)
- [x] 2.2 Delete the `src/HackerRank1.Api/` folder (contains only `obj/` output)
- [x] 2.3 Delete the root-level `bin/` and `obj/` folders
- [x] 2.4 Leave `HackerRank1/obj/` and the test projects' `bin/`/`obj/` folders untouched

## 3. Zero-touch verification

- [x] 3.1 Run `git diff --stat` and confirm no tracked source file under `HackerRank1/`, `LibraryService.Integration.Test/`, or `IntegrationTest/` was modified by this change
- [x] 3.2 Grep the change's own working diff (vs. the pre-change tree from bc71eda) to confirm zero namespace declarations or public type names were renamed

## 4. Build and test proof (per design D4)

- [x] 4.1 Run `dotnet build HackerRank1.sln` and confirm it succeeds without warnings-as-errors or new warnings in production code
- [ ] 4.2 Run `dotnet test LibraryService.Integration.Test/LibraryService.Integration.Test.csproj` and confirm all existing tests pass with the test files untouched (blocked: pre-existing EF Core 6 vs 8 version conflict; out-of-scope per user decision — see design.md Change Notes)
- [x] 4.3 Record build and test output summaries in the change notes as evidence for both "read-only test keeps compiling" and "no behavior change"

## 5. Spec contract on disk

- [x] 5.1 Confirm the capability file exists at `openspec/specs/vertical-slice-architecture/spec.md` after archive-time sync (review, not creation, at apply time) — confirmed: delta exists at `openspec/changes/vertical-slice-refactor/specs/vertical-slice-architecture/spec.md`; main spec is created only during the archive workflow