param(
    [string]$GamePath = "C:\Program Files (x86)\Steam\steamapps\common\Wicked Island",
    [string]$UE4SSZipPath = "$env:USERPROFILE\Downloads\UE4SS.zip",
    [string]$ModSourcePath = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($ModSourcePath)) {
    $ModSourcePath = Join-Path $repoRoot "mods\ue4ss\WickedIslandMods"
}

if (-not (Test-Path $GamePath)) {
    throw "Game path not found: $GamePath"
}

if (-not (Test-Path $UE4SSZipPath)) {
    throw "UE4SS zip not found: $UE4SSZipPath"
}

if (-not (Test-Path $ModSourcePath)) {
    throw "Mod source folder not found: $ModSourcePath"
}

$backupRoot = Join-Path $GamePath "_mod_backup"
$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupDir = Join-Path $backupRoot $stamp
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null

$trackedPaths = @(
    "Mods\WickedIslandMods",
    "ue4ss",
    "xinput1_3.dll",
    "dwmapi.dll",
    "version.dll"
)

foreach ($relativePath in $trackedPaths) {
    $sourcePath = Join-Path $GamePath $relativePath
    if (Test-Path $sourcePath) {
        $targetPath = Join-Path $backupDir $relativePath
        $targetParent = Split-Path -Parent $targetPath
        New-Item -ItemType Directory -Path $targetParent -Force | Out-Null
        Copy-Item -Path $sourcePath -Destination $targetPath -Recurse -Force
    }
}

$extractPath = Join-Path $env:TEMP ("ue4ss_extract_" + [Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $extractPath -Force | Out-Null
Expand-Archive -Path $UE4SSZipPath -DestinationPath $extractPath -Force

Copy-Item -Path (Join-Path $extractPath "*") -Destination $GamePath -Recurse -Force
Remove-Item -Path $extractPath -Recurse -Force

$modsRoot = Join-Path $GamePath "Mods"
$installPath = Join-Path $modsRoot "WickedIslandMods"
New-Item -ItemType Directory -Path $modsRoot -Force | Out-Null

if (Test-Path $installPath) {
    Remove-Item -Path $installPath -Recurse -Force
}

Copy-Item -Path $ModSourcePath -Destination $installPath -Recurse -Force

$statePath = Join-Path $GamePath "_wickedisland_mod_state.json"
$state = @{
    gamePath = $GamePath
    installPath = $installPath
    backupDir = $backupDir
    installedAtUtc = [DateTime]::UtcNow.ToString("o")
}
$state | ConvertTo-Json | Set-Content -Path $statePath -Encoding UTF8

Write-Host "Install complete." -ForegroundColor Green
Write-Host "Game path: $GamePath"
Write-Host "Mod path: $installPath"
Write-Host "Backup path: $backupDir"
