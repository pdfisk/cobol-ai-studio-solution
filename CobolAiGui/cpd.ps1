# Copy CobolAiGui published Blazor WASM wwwroot to CobolAiDesktop\wwwroot
# Run after: dotnet publish -c Debug (or Release)
#
# Usage:
#   .\copy-wwwroot-to-desktop.ps1
#   .\copy-wwwroot-to-desktop.ps1 -Configuration Release

param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'

$guiRoot = $PSScriptRoot
$src = Join-Path $guiRoot "bin\$Configuration\net10.0\browser-wasm\publish\wwwroot"
$repoRoot = Split-Path $guiRoot -Parent
$dst = Join-Path $repoRoot "CobolAiDesktop\wwwroot"

if (-not (Test-Path -LiteralPath $src -PathType Container)) {
    Write-Error "Source wwwroot not found: $src`nBuild CobolAiGui first, e.g.: dotnet publish -c $Configuration"
}

if (Test-Path -LiteralPath $dst) {
    Remove-Item -LiteralPath $dst -Recurse -Force
}

New-Item -ItemType Directory -Path $dst -Force | Out-Null
Copy-Item -Path (Join-Path $src '*') -Destination $dst -Recurse -Force

Write-Host "Copied wwwroot -> $dst"
