## 1. Structural audit (verification-first, per design D1)

- [x] 1.1 Enumerate every production `.cs` file under `src/` (excluding `obj/`) and record, in the change notes, the project and layer folder each file sits under
- [x] 1.2 Assert the solution contains exactly `src/HackerRank1.Domain`, `src/HackerRank1.Application`, `src/HackerRank1.Infrastructure`, `src/HackerRank1.Api` (production) plus `LibraryService.Integration.Test`; report any other project (e.g. a legacy flat project reference)
- [x] 1.3 Assert the `<ProjectReference>` graph is exactly `Api → Infrastructure → Application → Domain`: Domain has none, Application references only Domain, Infrastructure references only Application, Api references only Infrastructure
- [x] 1.4 Assert layer responsibilities: `Domain/Entities/` has only pure entities (no EF attributes); `Application/` has only contracts, DTOs, and `Common/Settings/`; `Infrastructure/` has `Persistence/` (+migrations), `Services/`, and `Auth/TokenGenerator.cs`; `Api/` has only `Controllers/` and hosting files (`Program.cs`, `Startup.cs`, `appsettings*`)
- [x] 1.5 Assert `src/HackerRank1.Api` never consumes Domain or Application types directly (controllers consume only Infrastructure-implemented interfaces)
- [x] 1.6 If the audit finds a violation, move the offending file to its correct layer folder without changing its namespace (or fix the offending project reference); confirm `git status` afterwards shows only that change — no violations found, no moves needed

## 2. Cleanup of stray layout leftover (per design D3)

- [x] 2.1 Verify `git ls-files` shows nothing tracked under the root `HackerRank1/` folder (untracked-only before deleting)
- [x] 2.2 Delete the root-level `HackerRank1/` leftover folder (empty directories + build output only)
- [x] 2.3 Leave `src/*/bin` and `src/*/obj` (live build output) untouched

## 3. Zero-touch verification (per design D2)

- [x] 3.1 Run `git diff --stat` and confirm no tracked source file under `src/`, `LibraryService.Integration.Test/`, or `IntegrationTest/` was modified by this change
- [x] 3.2 Grep the change's own working diff (vs. the pre-change tree from b565921) to confirm zero namespace declarations or public type names were renamed

## 4. Build and test proof (per design D4)

- [x] 4.1 Run `dotnet build HackerRank1.sln` and confirm it succeeds without warnings-as-errors or new warnings in production code — succeeded after the user-approved csproj fix (removed orphaned MSTest `<Using>`, pre-existing CS0234); 0 errors, 6 warnings
- [ ] 4.2 Run `dotnet test LibraryService.Integration.Test/LibraryService.Integration.Test.csproj` with the test files untouched; record the result (build/compile status and, if it runs, pass/fail) — a pre-existing EF Core 6-vs-8 runtime failure is expected and stays out of scope (blocked: compiles and runs; 3 tests fail with the expected pre-existing MissingMethodException — see design.md Change Notes)
- [x] 4.3 Record build and test output summaries in the change notes as evidence for "read-only test keeps compiling" and "no behavior change"

## 5. Spec contract on disk

- [x] 5.1 Confirm the capability file exists at `openspec/specs/clean-architecture/spec.md` after archive-time sync (review, not creation, at apply time) — confirmed: delta exists at `openspec/changes/clean-architecture-refactor/specs/clean-architecture/spec.md`; main spec is created only during the archive workflow