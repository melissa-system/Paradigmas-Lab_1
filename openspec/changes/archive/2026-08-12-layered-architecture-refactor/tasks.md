## Tasks

- [x] **Audit: project reference graph** — run `dotnet sln list` and read the four `src/*/*.csproj` files; record in the verification report that references are exactly `Api → BusinessLogic → DataAccess → Entities` with Entities referencing no project. (chore)
- [x] **Audit: layer folder placement** — list all tracked `.cs` files under `src/` and confirm each sits in its layer's folder (`Api/Controllers`, `Api` root hosting, `BusinessLogic/Services`, `BusinessLogic/Helpers`, `DataAccess/Data`, `DataAccess/Migrations`, `Entities/Models`, `Entities/DTO`, `Entities/Settings`). (chore)
- [x] **Verify: namespace preservation** — grep namespace declarations in `src/**/*.cs` and confirm the set (`LibraryService.WebAPI.*`, `LibraryService.WebAPI.Data`, `LibraryService.WebAPI.DTO`, `HackerRank1.*`) is unchanged vs. commit `d732b15`; confirm `LibraryService.Integration.Test/IntegrationTest.cs` and `IntegrationTest/IntegrationTest.cs` are byte-identical to `d732b15`. (chore)
- [x] **Verify: compile** — run `dotnet build HackerRank1.sln`; it SHALL succeed (test project compiles against preserved namespaces). (chore)
- [x] **Cleanup: stray leftovers** — for each untracked straggler folder from the previous flat layout: verify `git ls-files <folder>` is empty, then delete it; record each removal. (chore)
- [x] **Verify: no renames** — `git diff d732b15..HEAD -- src/` SHALL show only new-project moves/additions, no namespace or public type renames. (chore)

### Test Task

- [x] **Run integration tests (baseline, no fixing)** — `dotnet test HackerRank1.sln`; record result: pass, or fail with the known out-of-scope EF Core 6-vs-8 `MissingMethodException` runtime conflict. Do not modify test files or packages.