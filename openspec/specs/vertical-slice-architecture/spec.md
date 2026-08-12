# vertical-slice-architecture Specification

## Purpose

Defines the structural contract for organizing the HackerRank1 API project into vertical slices: feature use-cases live under `Features/<Feature>/<UseCase>/`, genuinely cross-cutting code under `Common/`, while legacy namespace names and the read-only integration tests are preserved unchanged.

## Requirements

### Requirement: Feature code organized in vertical slices
All production code in the `HackerRank1` project SHALL live under one of two roots: `Features/` for feature-specific code or `Common/` for code shared by multiple features. Under `Features/`, code SHALL be nested as `<Feature>/<UseCase>/`, where `UseCase` is the operation the code implements (e.g. `Login`, `GetLibraries`, `UpdateLibrary`). The project SHALL NOT contain the legacy layer folders `Controllers/`, `DTO/`, `Entities/`, `Helpers/`, `Services/`, or `Data/`.

#### Scenario: Every production file is under Features or Common
- **WHEN** enumerating all `.cs` files in the `HackerRank1` project that are part of the build
- **THEN** each file is under `Features/` or `Common/` and no legacy layer folder exists anywhere in the project tree

#### Scenario: New code follows the feature convention
- **WHEN** a developer adds an operation for an existing feature (e.g. a new library use-case)
- **THEN** the code is placed under `Features/<Feature>/<NewUseCase>/` rather than in a shared controller or service layer folder

### Requirement: Use-case slices are self-contained
Each `Features/<Feature>/<UseCase>/` folder SHALL contain every artifact that belongs to that use case and nothing else: its controller actions, request/response types, and service logic. Types used by exactly one use case SHALL live in that use case's folder. Code reused by multiple use-cases of the same feature SHALL live in `Features/<Feature>/Shared/`.

#### Scenario: Single-use types stay with their use case
- **WHEN** a request, response, or internal type is used by only one use-case
- **THEN** its source file sits inside that use-case's folder

#### Scenario: Shared feature code goes to a Shared subfolder
- **WHEN** code is used by more than one use-case of the same feature (e.g. the libraries service backing five CRUD slices)
- **THEN** it lives under `Features/<Feature>/Shared/`

### Requirement: Cross-cutting code confined to Common
Code that is genuinely transversal to all features SHALL live under `Common/` in purpose-named subfolders (e.g. `Common/Persistence/`, `Common/Settings/`). Feature-specific code MUST NOT be placed under `Common/`.

#### Scenario: Cross-cutting types are under Common
- **WHEN** inspecting the project tree
- **THEN** the only folders outside `Features/` in the production project are under `Common/` (persistence, migrations, and settings), and none of their files reference feature logic

### Requirement: Legacy namespaces preserved
The `LibraryService.Integration.Test/IntegrationTest.cs` test file depends on `LibraryService.WebAPI`, `LibraryService.WebAPI.Data`, and `LibraryService.WebAPI.DTO`, so the production code SHALL keep those namespaces and the `HackerRank1.*` namespaces (e.g. `HackerRank1.DTO`, `HackerRank1.Entities`, `HackerRank1.Services`, `HackerRank1.Controllers`, `HackerRank1.Helpers`, `HackerRank1.Migrations`) exactly as they are today. No namespace or type rename is permitted by this change.

#### Scenario: Read-only test keeps compiling
- **WHEN** building the solution as part of this change
- **THEN** `LibraryService.Integration.Test/IntegrationTest.cs` and `IntegrationTest/IntegrationTest.cs` compile unchanged against the preserved namespaces

#### Scenario: No namespace renames introduced
- **WHEN** diffing the production project across this change
- **THEN** no namespace declaration or public type name in `HackerRank1/` is altered

### Requirement: Integration tests are read-only
The files `LibraryService.Integration.Test/IntegrationTest.cs` and `IntegrationTest/IntegrationTest.cs` SHALL NOT be modified by this change, under any circumstance.

#### Scenario: Test file untouched
- **WHEN** applying this change
- **THEN** the two integration test files have zero changes applied by the implementation and still pass all existing tests

### Requirement: Repo free of stray build artifacts
Repository folders that contain only build output and belong to a layout that no longer exists SHALL be removed. At minimum, `src/HackerRank1.Api/` (containing only `obj/` output) and root-level `bin/` and `obj/` folders SHALL NOT exist after this change; build artifacts SHALL be confined to each project's own `bin/` and `obj/` folders.

#### Scenario: Stale artifact folders removed
- **WHEN** inspecting the repository root and `src/` after this change
- **THEN** `src/HackerRank1.Api/`, root `bin/`, and root `obj/` do not exist, while `HackerRank1/obj` (a live project's output folder) is allowed to remain