# wicked-island

mods for the game

## Included mod framework

This repo includes a BepInEx plugin scaffold plus two systems:

- in-scene **number-key pose switching** with on-screen list
- **universal entity interaction policy** with allow-by-default rules and global tag blocks

### Files

- `/home/runner/work/wicked-island/wicked-island/src/WickedIsland.Mods/WickedIsland.Mods.csproj`
- `/home/runner/work/wicked-island/wicked-island/src/WickedIsland.Mods/WickedIslandModsPlugin.cs`
- `/home/runner/work/wicked-island/wicked-island/src/WickedIsland.Mods/Interaction/PoseSwitchSystem.cs`
- `/home/runner/work/wicked-island/wicked-island/src/WickedIsland.Mods/Interaction/UniversalEntityInteractionPolicy.cs`
- `/home/runner/work/wicked-island/wicked-island/mods/config/interaction-framework.example.json`

## Build (BepInEx target)

Set paths to your local game/mod-loader install:

- `BepInExDir` (or env var `BEPINEX_DIR`) -> your game's `BepInEx` folder
- `UnityManagedDir` (or env var `WICKED_ISLAND_MANAGED_DIR`) -> `Wicked Island*_Data/Managed`

Example:

```bash
dotnet build /home/runner/work/wicked-island/wicked-island/src/WickedIsland.Mods/WickedIsland.Mods.csproj -c Release -p:BepInExDir="D:\SteamLibrary\steamapps\common\Wicked Island\BepInEx" -p:UnityManagedDir="D:\SteamLibrary\steamapps\common\Wicked Island\Wicked Island_Data\Managed"
```

Output DLL:

- `/home/runner/work/wicked-island/wicked-island/src/WickedIsland.Mods/bin/Release/net472/WickedIsland.Mods.dll`

## Install to test (Steam)

1. Install BepInEx into Wicked Island.
2. Launch the game once, then close it (this creates BepInEx folders).
3. Copy plugin DLL to:
   - `...\Wicked Island\BepInEx\plugins\WickedIsland.Mods.dll`
4. (Optional) Copy config example:
   - from `/home/runner/work/wicked-island/wicked-island/mods/config/interaction-framework.example.json`
   - to `...\Wicked Island\BepInEx\config\interaction-framework.json`
5. Start the game and check:
   - BepInEx log contains `Wicked Island Mods 0.1.0 loaded.`
   - Number keys `1..0` switch poses in the demo scene context.
   - `F7` writes sample universal entity eligibility result in logs.
   - Overlay appears in top-left when enabled.

## Runtime notes

- Current plugin entrypoint uses a demo context (`DemoSceneInteractionContext`) so you can verify install and key handling immediately.
- Replace demo context with real game hooks in `WickedIslandModsPlugin`:
  - implement `ISceneInteractionContext` against Wicked Island scene state
  - call `UniversalEntityInteractionPolicy.IsEligible(...)` inside your actual interaction gating path
