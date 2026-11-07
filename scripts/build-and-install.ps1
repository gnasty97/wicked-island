param(
    [string]$GamePath = "C:\Program Files (x86)\Steam\steamapps\common\Wicked Island"
)

$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ProjectPath = Join-Path $RepoRoot "src\WickedIsland.Mods\WickedIsland.Mods.csproj"
$ManagedDir = Join-Path $GamePath "Wicked Island_Data\Managed"
$BepInExDir = Join-Path $GamePath "BepInEx"
$PluginOutDir = Join-Path $RepoRoot "src\WickedIsland.Mods\bin\Release\net472"
$PluginDll = Join-Path $PluginOutDir "WickedIsland.Mods.dll"
$PluginInstallDir = Join-Path $BepInExDir "plugins"
$ConfigSource = Join-Path $RepoRoot "mods\config\interaction-framework.example.json"
$ConfigInstallDir = Join-Path $BepInExDir "config"
$ConfigInstallPath = Join-Path $ConfigInstallDir "interaction-framework.json"

if (-not (Test-Path $ProjectPath)) {
    throw "Project file not found: $ProjectPath"
}

if (-not (Test-Path $ManagedDir)) {
    throw "Managed directory not found: $ManagedDir"
}

if (-not (Test-Path $BepInExDir)) {
    throw "BepInEx directory not found: $BepInExDir"
}

Write-Host "Building plugin..." -ForegroundColor Cyan
dotnet build $ProjectPath -c Release -p:BepInExDir="$BepInExDir" -p:UnityManagedDir="$ManagedDir"

if (-not (Test-Path $PluginDll)) {
    throw "Build output not found: $PluginDll"
}

New-Item -ItemType Directory -Path $PluginInstallDir -Force | Out-Null
Copy-Item -Path $PluginDll -Destination (Join-Path $PluginInstallDir "WickedIsland.Mods.dll") -Force

if (Test-Path $ConfigSource) {
    New-Item -ItemType Directory -Path $ConfigInstallDir -Force | Out-Null
    Copy-Item -Path $ConfigSource -Destination $ConfigInstallPath -Force
}

Write-Host "Done." -ForegroundColor Green
Write-Host "Installed plugin to: $(Join-Path $PluginInstallDir "WickedIsland.Mods.dll")"
Write-Host "Installed config to: $ConfigInstallPath"
