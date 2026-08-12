## Why

The codebase is mid-transition between a legacy layered layout (Controllers/, DTO/, Entities/, Helpers/, Services/) and vertical slices. Commit `bc71eda` moved the folders into `Features/` + `Common/`, but there is no formal contract describing the target structure, and the codebase still carries residual inconsistencies from the first pass (e.g. files whose namespace implies a layer folder that no longer exists, stray build-artifact folders). This change codifies the vertical-slice structure as a verifiable capability and completes the structural cleanup — without renaming namespaces and without any runtime behavior change.

## What Changes

- Add a `vertical-slice-architecture` capability spec that establishes the structural contract: feature use-cases live under `HackerRank1/Features/<Feature>/<UseCase>/`, cross-cutting code under `HackerRank1/Common/`, each use-case slice is self-contained (controller action, request/response types, service logic together), and shared code is placed under a `Shared/` subfolder of the feature.
- Apply the contract to the remaining inconsistencies:
  - Verify every production `.cs` file sits under `Features/` or `Common/` and that no layer-style folders (`Controllers/`, `DTO/`, `Entities/`, `Helpers/`, `Services/`, `Data/`) exist in the project tree.
  - Remove stray build-artifact folders that are not part of the project (`src/HackerRank1.Api/` containing only `obj/`, root-level `bin/` and `obj/`).
  - Confirm `Common/` holds only truly cross-cutting code (`Persistence/`, `Settings/`, `Migrations/`).
- Namespaces are preserved verbatim (`LibraryService.WebAPI.*`, `HackerRank1.*`, `HackerRank1.Migrations`) — no namespace renames, no type moves.
- `LibraryService.Integration.Test/IntegrationTest.cs` stays read-only and untouched; it depends on those namespace names and must keep compiling.

## Capabilities

### New Capabilities
- `vertical-slice-architecture`: structural contract for organizing the HackerRank1 API project — feature slices under `Features/<Feature>/<UseCase>/`, cross-cutting code under `Common/`, self-contained use-case slices, preserved legacy namespaces, and the read-only integration test constraint.

### Modified Capabilities
<!-- None: `openspec/specs/` is empty and no runtime behavior changes. -->

## Impact

- **Code**: `HackerRank1/` production project tree structure (folders and file placement only; no file contents changed beyond verification).
- **APIs**: none — endpoints, routes, models, and namespaces unchanged.
- **Tests**: `LibraryService.Integration.Test` and `IntegrationTest` projects are not modified; they must still build and pass.
- **Repositories/build**: deletion of stale build-artifact folders (`src/HackerRank1.Api/obj`, root `bin/`, `obj/`); no dependency changes.