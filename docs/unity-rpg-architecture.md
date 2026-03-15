# OpenMM8 Unity Architecture

This project is in a migration phase from an older Unity layout to a cleaner Unity 2022.3 structure. The immediate goal is to stop adding more logic to global manager classes and move toward feature-oriented runtime code.

## Current priority

Functional progress takes priority over architectural purity.

- Primary milestone: 100% Dagger Wound Island playability
- Secondary goal: keep architecture moving toward cleaner Unity runtime boundaries while supporting that milestone
- Defer non-blocking cleanup if it does not directly help island completeness, testability, or maintenance of island work

## Target structure

Use this as the direction for incremental moves, not a big-bang rewrite:

```text
Assets/OpenMM8/
  Runtime/
    Bootstrap/
    Core/
    Data/
    Features/
      Characters/
      Combat/
      Dialogue/
      Inventory/
      Quests/
      World/
    UI/
  Editor/
  Tests/
  Content/
    Art/
    Audio/
    Prefabs/
    Resources/
    Scenes/
  Vendor/
```

## Rules for new code

1. Put gameplay code under `Runtime`, grouped by feature rather than by technical type.
2. Keep editor tooling under `Editor` only.
3. Keep third-party or imported code under `Vendor` and avoid editing it unless necessary.
4. Prefer serialized configuration over hard-coded bootstrap lists and debug data.
5. Keep scene bootstrap thin: compose services, then hand off to feature systems.
6. Split runtime code with assembly definitions once vendor code is isolated enough to make references explicit.

## Migration order

1. Move Unity Standard Assets and third-party packages out of `Assets/OpenMM8/Scripts` into a dedicated `Vendor` area.
2. Create runtime/editor asmdefs after vendor/runtime boundaries are clean.
3. Replace singleton-heavy bootstrap with a small startup pipeline:
   - database load
   - scene references bind
   - gameplay systems initialize
   - debug/dev-only seed data applies
4. Break `GameCore` into narrower systems:
   - startup/bootstrap
   - input routing
   - inspection/interaction
   - debug tooling
5. Convert hard-coded data tables and startup lists into `ScriptableObject` configs where they are designer-owned.

## Current first-pass cleanup

This repository pass establishes four conventions:

- generated Unity/editor output should stay out of version control
- unnecessary Unity online-service packages should not stay installed by default
- imported/vendor code should live outside the gameplay tree
- `GameCore` should keep runtime orchestration only, while startup/bootstrap lives in `InitMgr`

## Current vendor boundary

Imported code has been moved under `Assets/OpenMM8/Vendor` so the project can separate ownership before asmdefs are introduced.

- `Vendor/UnityStandardAssets`: Unity sample code still referenced by gameplay
- `Vendor/ThirdParty`: active external libraries
- `Vendor/Debug`: debug-only tools/plugins
- `Vendor/Legacy`: duplicate or partial imports awaiting review/removal

## Current assembly split

The project now uses an explicit first-pass assembly layout:

- `OpenMM8.Runtime`: gameplay/runtime code under `Assets/OpenMM8/Scripts`
- `OpenMM8.Editor`: editor tooling under `Assets/Editor`
- `OpenMM8.Tests`: editor test assembly
- `OpenMM8.Vendor.UnityStandardAssets`: imported runtime helpers still used by gameplay
- `OpenMM8.Vendor.UnityStandardAssets.Editor`: editor-only helpers for that vendor package
- `OpenMM8.Vendor.IngameDebugConsole`: runtime debug console integration

This is intentionally a shallow split. The next step is to create feature assemblies inside runtime once the current gameplay code is no longer centered around global managers.

## Current bootstrap split

Startup responsibilities are no longer mixed into `GameCore`.

- `InitMgr`: database boot, config-driven sprite preloading, manager initialization order, post-init hook, debug party seeding
- `GameCore`: runtime state, global input routing, inspection state, debug hotkeys
- `PartyRosterService`: transitional helper for party creation, debug seeding, and placeholder roster recruitment flows

The current startup defaults now live in a `GameStartupConfig` asset under `Resources/Configs`, with `InitMgr` auto-loading it when no scene reference is assigned.

This is still transitional. The next cleanup should split `PartyRosterService` into real recruitment/bootstrap flows instead of one helper, then move the bootstrap config out of `Resources` once scene composition is cleaner.

## Deferred TODOs

- Split debug-only startup data out of `GameStartupConfig` into a separate dev/debug config when it starts getting in the way of gameplay work.
