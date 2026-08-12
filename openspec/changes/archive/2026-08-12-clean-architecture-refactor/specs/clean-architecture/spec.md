## Purpose

Defines the structural contract for the multi-project Clean Architecture layout of the HackerRank1 solution: four layers under `src/` with one-directional dependencies, preserved legacy namespaces, and read-only integration tests that must keep compiling unchanged.

## ADDED Requirements

### Requirement: Four-layer project organization
The solution SHALL contain exactly four production projects under `src/`: `HackerRank1.Domain`, `HackerRank1.Application`, `HackerRank1.Infrastructure`, and `HackerRank1.Api`, plus the read-only `LibraryService.Integration.Test` test project. The solution SHALL NOT reference the legacy flat `HackerRank1/` project.

#### Scenario: Solution lists only the four src projects plus the test project
- **WHEN** enumerating the projects in `HackerRank1.sln`
- **THEN** the production projects are exactly `src/HackerRank1.Domain`, `src/HackerRank1.Application`, `src/HackerRank1.Infrastructure`, and `src/HackerRank1.Api`, and the only test project is `LibraryService.Integration.Test`

### Requirement: One-directional dependency rule
Project references SHALL follow exactly the chain `Api → Infrastructure → Application → Domain`: `HackerRank1.Domain` SHALL reference no other project, `HackerRank1.Application` SHALL reference only `HackerRank1.Domain`, `HackerRank1.Infrastructure` SHALL reference only `HackerRank1.Application`, and `HackerRank1.Api` SHALL reference only `HackerRank1.Infrastructure`. No reverse or sideways references are permitted.

#### Scenario: Dependency graph matches the chain
- **WHEN** inspecting the `<ProjectReference>` entries of all four `src/*/*.csproj` files
- **THEN** Domain has none, Application references only Domain, Infrastructure references only Application, and Api references only Infrastructure

#### Scenario: No reference crosses layers
- **WHEN** compiling the solution
- **THEN** no production file consumes types from a project that is not its direct upstream per the chain (e.g. Api never uses Domain or Application types directly)

### Requirement: Layer-specific responsibilities
Each project SHALL contain only code matching its layer role:
- `HackerRank1.Domain` SHALL hold pure entities only (e.g. `Entities/Library.cs`, `Entities/Book.cs`) with no EF attributes or framework dependencies.
- `HackerRank1.Application` SHALL hold service contracts (interfaces) and feature DTOs (e.g. `Auth/`, `Books/`, `Libraries/`, each with `Dtos/`), plus cross-cutting settings under `Common/Settings/`.
- `HackerRank1.Infrastructure` SHALL hold persistence (`Persistence/` with `LibraryContext` and migrations), service implementations (`Services/`), and token generation (`Auth/TokenGenerator.cs`).
- `HackerRank1.Api` SHALL hold controllers (`Controllers/`) and hosting code (`Program.cs`, `Startup.cs`, `appsettings*`).

#### Scenario: Entities are infrastructure-free
- **WHEN** inspecting the Domain project
- **THEN** it contains only entity classes and references no EF Core, ASP.NET, or other framework package, and its entities carry no EF attributes

#### Scenario: Contracts and implementations are separated
- **WHEN** locating a service interface (e.g. `ILibrariesService`, `IBooksService`, `IAuthenticationService`)
- **THEN** it is declared in `HackerRank1.Application` and implemented in `HackerRank1.Infrastructure`, and `HackerRank1.Api` consumes the interface, never the implementation directly

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
The root-level `HackerRank1/` folder from the previous flat-project layout SHALL NOT exist after this change; it contains no tracked files (only empty directories and build output) and is a leftover of the pre-clean-architecture project. Build artifacts SHALL live only under each project's own `bin/` and `obj/` folders.

#### Scenario: Stale root folder removed
- **WHEN** inspecting the repository root after this change
- **THEN** `HackerRank1/` does not exist at the root, while each `src/<Project>/bin` and `src/<Project>/obj` (live build output) may exist