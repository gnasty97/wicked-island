# wicked-island

mods for the game

## Recommended path (Unreal/UE4SS)

This repository now includes:

- UE4SS install script
- UE4SS uninstall/rollback script
- Ready-to-drop UE4SS mod template folder (`WickedIslandMods`)

### Key files

- `/home/runner/work/wicked-island/wicked-island/scripts/install-ue4ss-mod.ps1`
- `/home/runner/work/wicked-island/wicked-island/scripts/uninstall-ue4ss-mod.ps1`
- `/home/runner/work/wicked-island/wicked-island/mods/ue4ss/WickedIslandMods/enabled.txt`
- `/home/runner/work/wicked-island/wicked-island/mods/ue4ss/WickedIslandMods/scripts/main.lua`

## Install

1. Download a UE4SS release zip and place it at:
   - `C:\Users\<you>\Downloads\UE4SS.zip`
2. Run:

```powershell
powershell -ExecutionPolicy Bypass -File /home/runner/work/wicked-island/wicked-island/scripts/install-ue4ss-mod.ps1
```

### Optional parameters

```powershell
powershell -ExecutionPolicy Bypass -File /home/runner/work/wicked-island/wicked-island/scripts/install-ue4ss-mod.ps1 `
  -GamePath "C:\Program Files (x86)\Steam\steamapps\common\Wicked Island" `
  -UE4SSZipPath "C:\Users\<you>\Downloads\UE4SS_vX.Y.Z.zip" `
  -ModSourcePath "C:\path\to\custom\WickedIslandMods"
```

The installer:

- backs up existing loader/mod files into `_mod_backup\<timestamp>`
- installs UE4SS files into the game directory
- installs `WickedIslandMods` into `...\Wicked Island\Mods\WickedIslandMods`
- writes install state to `...\Wicked Island\_wickedisland_mod_state.json`

## Uninstall / rollback

```powershell
powershell -ExecutionPolicy Bypass -File /home/runner/work/wicked-island/wicked-island/scripts/uninstall-ue4ss-mod.ps1
```

This removes `Mods\WickedIslandMods` and restores any backed-up files.

## Mod template notes

`scripts/main.lua` currently provides scaffold logic:

- Number-based pose switching function (`switchPoseByNumber`)
- Overlay text generation function (`renderOverlay`)
- Universal entity eligibility policy function (`isEntityEligible`)

Replace the marked stubs with game-specific hooks and UI calls.

## Legacy C# scaffold

The repository still contains an earlier C# BepInEx scaffold under:

- `/home/runner/work/wicked-island/wicked-island/src/WickedIsland.Mods`

Use the UE4SS path above for this game runtime.
