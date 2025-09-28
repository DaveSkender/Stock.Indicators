#!/usr/bin/env pwsh

# Create new feature script for Stock Indicators (PowerShell version)
# Generates branch name and spec file path for new indicator features

param(
    [string]$Json = "",
    [Parameter(ValueFromRemainingArguments=$true)]
    [string[]]$Args
)

# Parse arguments
if ($Json) {
    $Arguments = $Json
} else {
    $Arguments = $Args -join " "
}

# Generate branch name from feature description
$BranchName = "feature/" + ($Arguments -replace '[^a-zA-Z0-9 ]', '' -replace ' +', '-' -replace '-+', '-').ToLower().Substring(0, [Math]::Min(50, $Arguments.Length))
$BranchName = $BranchName.TrimEnd('-')

# Ensure we're in the repo root
$RepoRoot = git rev-parse --show-toplevel
Set-Location $RepoRoot

# Create specs directory if it doesn't exist
$SpecsDir = Join-Path $RepoRoot "spec-kit/specs"
if (!(Test-Path $SpecsDir)) {
    New-Item -ItemType Directory -Path $SpecsDir -Force | Out-Null
}

# Generate spec file path
$SpecFileName = ($BranchName -replace '^feature/', '') + '.md'
$SpecFile = Join-Path $SpecsDir $SpecFileName

# Create and checkout new branch
try {
    git checkout -b $BranchName 2>$null
} catch {
    git checkout $BranchName 2>$null
}

# Initialize spec file if it doesn't exist
if (!(Test-Path $SpecFile)) {
    New-Item -ItemType File -Path $SpecFile -Force | Out-Null
}

# Output JSON for the spec-kit command
$Result = @{
    BRANCH_NAME = $BranchName
    SPEC_FILE = $SpecFile
} | ConvertTo-Json -Compress

Write-Output $Result