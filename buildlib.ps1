# Build script for CobolAiLib
param(
    [string]$Configuration = "Debug",
    [string]$Framework = "net10.0"
)

$ProjectPath = Join-Path $PSScriptRoot "CobolAiLib\CobolAiLib.csproj"

Write-Host "Building CobolAiLib ($Configuration)..." -ForegroundColor Cyan

dotnet build $ProjectPath --configuration $Configuration --framework $Framework

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build succeeded." -ForegroundColor Green
} else {
    Write-Host "Build failed." -ForegroundColor Red
    exit $LASTEXITCODE
}
