[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$RuntimeIdentifier = 'win-x64',

    [string]$ProjectPath = 'coords/coords.csproj',

    [string]$OutputDir = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot

if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    $OutputDir = Join-Path $repoRoot ("coords/bin/{0}" -f $Configuration)
}

$fullProjectPath = Join-Path $repoRoot $ProjectPath
if (-not (Test-Path $fullProjectPath)) {
    throw "Project not found: $fullProjectPath"
}

Write-Host "Publishing single-file exe..." -ForegroundColor Cyan
Write-Host "  Project: $fullProjectPath"
Write-Host "  Config : $Configuration"
Write-Host "  RID    : $RuntimeIdentifier"
Write-Host "  Output : $OutputDir (exe only)"

$tempPublishDir = Join-Path $OutputDir '.publish-tmp'
if (Test-Path $tempPublishDir) {
    Remove-Item -Recurse -Force $tempPublishDir
}
New-Item -ItemType Directory -Path $tempPublishDir | Out-Null

& dotnet publish $fullProjectPath `
    -c $Configuration `
    -r $RuntimeIdentifier `
    -p:SelfContained=true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $tempPublishDir

$publishedExe = Join-Path $tempPublishDir 'Coords.exe'
$finalExe = Join-Path $OutputDir 'Coords.exe'

if (-not (Test-Path $publishedExe)) {
    throw "Publish completed but exe not found at: $publishedExe"
}

Copy-Item -Force $publishedExe $finalExe
Remove-Item -Recurse -Force $tempPublishDir

Write-Host "OK: $finalExe" -ForegroundColor Green
