# wicked-island

mods for the game

## Included mod framework

This repo now includes a starter mod framework focused on:

- in-scene **number-key pose switching** with an on-screen list
- a **universal entity interaction policy** that can be extended per entity type

### Files

- `/home/runner/work/wicked-island/wicked-island/src/WickedIsland.Mods/Interaction/PoseSwitchSystem.cs`
- `/home/runner/work/wicked-island/wicked-island/src/WickedIsland.Mods/Interaction/UniversalEntityInteractionPolicy.cs`
- `/home/runner/work/wicked-island/wicked-island/mods/config/interaction-framework.example.json`

### How to use

1. Wire your game hooks to `PoseSwitchSystem` by implementing `ISceneInteractionContext`.
2. Call `HandleNumberKeyPress(...)` while a scene is active.
3. Render `BuildOverlayText(...)` in your HUD/UI.
4. Use `UniversalEntityInteractionPolicy` to centralize whether an entity is eligible for interactions.
5. Adjust defaults in `interaction-framework.example.json` and load into your own runtime config object.
