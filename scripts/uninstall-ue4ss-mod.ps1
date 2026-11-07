param(
    [string]$GamePath = "C:\Program Files (x86)\Steam\steamapps\common\Wicked Island"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $GamePath)) {
    throw "Game path not found: $GamePath"
}

$statePath = Join-Path $GamePath "_wickedisland_mod_state.json"
if (-not (Test-Path $statePath)) {
    throw "Install state file not found: $statePath"
}

$state = Get-Content -Path $statePath | ConvertFrom-Json
if (-not $state.backupDir) {
    throw "Invalid state file. Missing backupDir."
}

$installPath = Join-Path $GamePath "Mods\WickedIslandMods"
if (Test-Path $installPath) {
    Remove-Item -Path $installPath -Recurse -Force
}

$restoreTargets = @(
    "Mods\WickedIslandMods",
    "ue4ss",
    "xinput1_3.dll",
    "dwmapi.dll",
    "version.dll"
)

foreach ($relativePath in $restoreTargets) {
    $destinationPath = Join-Path $GamePath $relativePath
    $backupPath = Join-Path $state.backupDir $relativePath

    if (Test-Path $destinationPath) {
        Remove-Item -Path $destinationPath -Recurse -Force -ErrorAction SilentlyContinue
    }

    if (Test-Path $backupPath) {
        $destinationParent = Split-Path -Parent $destinationPath
        New-Item -ItemType Directory -Path $destinationParent -Force | Out-Null
        Copy-Item -Path $backupPath -Destination $destinationPath -Recurse -Force
    }
}

Write-Host "Uninstall complete." -ForegroundColor Green
Write-Host "Restored from backup: $($state.backupDir)"
