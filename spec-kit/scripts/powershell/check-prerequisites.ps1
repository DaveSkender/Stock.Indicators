#!/usr/bin/env pwsh

# Check prerequisites script for Stock Indicators tasks generation (PowerShell version)
# Generates feature directory and available documents list

param(
    [switch]$Json
)

# Get repository root
$RepoRoot = git rev-parse --show-toplevel
Set-Location $RepoRoot

# Feature directory (for v3 streaming indicators)
$FeatureDir = Join-Path $RepoRoot "spec-kit/specs"

# Check for available design documents
$AvailableDocs = @()

# Check for core specification
$SpecFile = Join-Path $FeatureDir "v3-streaming-indicators.md"
if (Test-Path $SpecFile) {
    $AvailableDocs += "v3-streaming-indicators.md"
}

# Check for constitution
$ConstitutionFile = Join-Path $RepoRoot "spec-kit/memory/constitution.md"
if (Test-Path $ConstitutionFile) {
    $AvailableDocs += "constitution.md"
}

# Check for existing tasks
$TasksFile = Join-Path $FeatureDir "v3-streaming-tasks.md"
if (Test-Path $TasksFile) {
    $AvailableDocs += "v3-streaming-tasks.md"
}

# Output results
if ($Json) {
    # Generate JSON output for spec-kit command
    $Result = @{
        FEATURE_DIR = $FeatureDir
        AVAILABLE_DOCS = $AvailableDocs
    } | ConvertTo-Json -Compress
    
    Write-Output $Result
} else {
    # Human-readable output
    Write-Output "Feature Directory: $FeatureDir"
    Write-Output "Available Documents:"
    foreach ($doc in $AvailableDocs) {
        Write-Output "  - $doc"
    }
}