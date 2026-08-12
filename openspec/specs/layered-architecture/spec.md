## Purpose

Defines the structural contract for the N-Layer (layered) architecture of the HackerRank1 solution: four projects under `src/` with one-directional dependencies, preserved legacy namespaces, and read-only integration tests that must keep compiling unchanged.

## Requirements

### Requirement: Four-layer project organization
The solution SHALL contain exactly four production projects under `src/`: `HackerRank1.Api`, `HackerRank1.BusinessLogic`, `HackerRank1.DataAccess`, and `HackerRank1.Entities`, plus the read-only `LibraryService.Integration.Test` test project. The solution SHALL NOT reference the legacy flat `HackerRank1/` project.

#### Scenario: Solution lists only the four src projects plus the test project
- **WHEN** enumerating the projects in `HackerRank1.sln`
- **THEN** the production projects are exactly `src/HackerRank1.Api`, `src/HackerRank1.BusinessLogic`, `src/HackerRank1.DataAccess`, and `src/HackerRank1.Entities`, and the only test project is `LibraryService.Integration.Test`

### Requirement: One-directional dependency rule
Project references SHALL follow exactly the chain `Api → BusinessLogic → DataAccess → Entities`: `HackerRank1.Entities` SHALL reference no other project, `HackerRank1.DataAccess` SHALL reference only `HackerRank1.Entities`, `HackerRank1.BusinessLogic` SHALL reference only `HackerRank1.DataAccess`, and `HackerRank1.Api` SHALL reference only `HackerRank1.BusinessLogic`. No reverse or sideways references are permitted.

#### Scenario: Dependency graph matches the chain
- **WHEN** inspecting the `<ProjectReference>` entries of all four `src/*/*.csproj` files
- **THEN** Entities has none, DataAccess references only Entities, BusinessLogic references only DataAccess, and Api references only BusinessLogic

#### Scenario: No reference crosses layers
- **WHEN** compiling the solution
- **THEN** no production file consumes types from a project that is not its direct upstream per the chain (e.g. Api never uses DataAccess or Entities types directly)

### Requirement: Layer-specific responsibilities
Each project SHALL contain only code matching its layer role:
- `HackerRank1.Entities` SHALL hold models, DTOs, and settings only (e.g. `Models/`, `DTO/`, `Settings/`) with no framework or persistence dependencies.
- `HackerRank1.DataAccess` SHALL hold persistence only (`Data/LibraryContext.cs` and `Migrations/`) and reference only Entities.
- `HackerRank1.BusinessLogic` SHALL hold service implementations and helpers (`Services/`, `Helpers/TokenGenerator.cs`).
- `HackerRank1.Api` SHALL hold controllers (`Controllers/`) and hosting code (`Program.cs`, `Startup.cs`, `appsettings*`).

#### Scenario: Entities are persistence-free
- **WHEN** inspecting the Entities project
- **THEN** it contains only models, DTOs, and settings and references no EF Core, ASP.NET, or data-access package

#### Scenario: Services live in BusinessLogic, not Api
- **WHEN** locating a service implementation (e.g. `LibrariesService`, `BooksService`, `AuthenticationService`)
- **THEN** it is declared in `HackerRank1.BusinessLogic` and consumed by `HackerRank1.Api` through the reference chain

### Requirement: Legacy namespaces preserved
The `LibraryService.Integration.Test/IntegrationTest.cs` test file depends on `LibraryService.WebAPI`, `LibraryService.WebAPI.Data`, and `LibraryService.WebAPI.DTO`, so the production code SHALL keep those namespaces and the `HackerRank1.*` namespaces (e.g. `HackerRank1.DTO`, `HackerRank1.Entities`, `HackerRank1.Services`, `HackerRank1.Controllers`, `HackerRank1.Helpers`, `HackerRank1.Migrations`) exactly as they are today. No namespace or type rename is permitted by this change.

#### Scenario: Read-only test keeps compiling
- **WHEN** building the solution as part of this change
- **THEN** `LibraryService.Integration.Test/IntegrationTest.cs` and `IntegrationTest/IntegrationTest.cs` compile unchanged against the preserved namespaces

#### Scenario: No namespace renames introduced
- **WHEN** diffing the production projects across this change
- **THEN** no namespace declaration or public type name in `src/` is altered

### Requirement: Integration tests are read-only
The files `LibraryService.Integration.Test/IntegrationTest.cs` and `IntegrationTest/IntegrationTest.cs` SHALL NOT be modified by this change, under any circumstance. The legacy `IntegrationTest/` project (not part of the solution) SHALL remain untouched.

#### Scenario: Test files untouched
- **WHEN** applying this change
- **THEN** the two integration test files and the legacy `IntegrationTest/` project have zero changes applied by the implementation and the read-only test project still compiles

### Requirement: Repo free of stray layout leftovers
Stray folders from the previous flat-project layout SHALL NOT exist after this change unless they contain tracked content in use. Build artifacts SHALL live only under each project's own `bin/` and `obj/` folders.

#### Scenario: Stale leftover folders removed
- **WHEN** inspecting the repository root after this change
- **THEN** any untracked leftover folders from the pre-N-Layer project (verified file-free and unversioned) are removed, while each `src/<Project>/bin` and `src/<Project>/obj` (live build output) may exist