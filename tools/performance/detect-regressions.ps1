#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Performance regression detection script for Stock Indicators benchmarks.

.DESCRIPTION
    Compares current benchmark results with baseline to detect performance regressions.
    Uses JSON output from BenchmarkDotNet to perform statistical analysis.

.PARAMETER BaselineFile
    Path to baseline JSON results file.

.PARAMETER CurrentFile
    Path to current JSON results file.

.PARAMETER ThresholdPercent
    Percentage threshold for regression detection (default: 10%).

.EXAMPLE
    ./detect-regressions.ps1 -BaselineFile baseline.json -CurrentFile current.json -ThresholdPercent 10
#>

param(
    [Parameter(Mandatory = $false)]
    [string]$BaselineFile = "",
    
    [Parameter(Mandatory = $false)]
    [string]$CurrentFile = "",
    
    [Parameter(Mandatory = $false)]
    [double]$ThresholdPercent = 10.0
)

function Compare-BenchmarkResults {
    param(
        [Parameter(Mandatory = $true)]
        [PSCustomObject]$Baseline,
        
        [Parameter(Mandatory = $true)]
        [PSCustomObject]$Current,
        
        [Parameter(Mandatory = $true)]
        [double]$Threshold
    )
    
    $regressions = @()
    $improvements = @()
    
    # Create a lookup for baseline results
    $baselineDict = @{}
    foreach ($bench in $Baseline.Benchmarks) {
        $key = "$($bench.Type).$($bench.Method)"
        $baselineDict[$key] = $bench
    }
    
    # Compare current results with baseline
    foreach ($bench in $Current.Benchmarks) {
        $key = "$($bench.Type).$($bench.Method)"
        
        if ($baselineDict.ContainsKey($key)) {
            $baselineBench = $baselineDict[$key]
            $baselineMean = [double]$baselineBench.Statistics.Mean
            $currentMean = [double]$bench.Statistics.Mean
            
            if ($baselineMean -gt 0) {
                $percentChange = (($currentMean - $baselineMean) / $baselineMean) * 100
                
                if ($percentChange -gt $Threshold) {
                    $regressions += [PSCustomObject]@{
                        Benchmark = $key
                        BaselineMean = $baselineMean
                        CurrentMean = $currentMean
                        PercentChange = [math]::Round($percentChange, 2)
                    }
                }
                elseif ($percentChange -lt -$Threshold) {
                    $improvements += [PSCustomObject]@{
                        Benchmark = $key
                        BaselineMean = $baselineMean
                        CurrentMean = $currentMean
                        PercentChange = [math]::Round($percentChange, 2)
                    }
                }
            }
        }
    }
    
    return @{
        Regressions = $regressions
        Improvements = $improvements
    }
}

# Main execution
Write-Host "Performance Regression Detection" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

# Find latest result files if not specified
if ([string]::IsNullOrEmpty($CurrentFile)) {
    $resultsDir = "BenchmarkDotNet.Artifacts/results"
    if (Test-Path $resultsDir) {
        $jsonFiles = Get-ChildItem -Path $resultsDir -Filter "*.json" -File | 
                     Sort-Object LastWriteTime -Descending
        
        if ($jsonFiles.Count -ge 1) {
            $CurrentFile = $jsonFiles[0].FullName
            Write-Host "Using current results: $CurrentFile" -ForegroundColor Yellow
        }
    }
}

if ([string]::IsNullOrEmpty($BaselineFile)) {
    $baselineDir = "baselines"
    if (Test-Path $baselineDir) {
        $baselineFiles = Get-ChildItem -Path $baselineDir -Filter "*.json" -File |
                        Sort-Object LastWriteTime -Descending
        
        if ($baselineFiles.Count -ge 1) {
            $BaselineFile = $baselineFiles[0].FullName
            Write-Host "Using baseline: $BaselineFile" -ForegroundColor Yellow
        }
    }
}

if ([string]::IsNullOrEmpty($BaselineFile) -or [string]::IsNullOrEmpty($CurrentFile)) {
    Write-Host "Error: Baseline or current results file not found." -ForegroundColor Red
    Write-Host "Please specify paths manually or ensure files exist in expected locations." -ForegroundColor Red
    Write-Host ""
    Write-Host "Usage: ./detect-regressions.ps1 -BaselineFile <path> -CurrentFile <path> [-ThresholdPercent <percent>]" -ForegroundColor Yellow
    exit 1
}

if (-not (Test-Path $BaselineFile)) {
    Write-Host "Error: Baseline file not found: $BaselineFile" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $CurrentFile)) {
    Write-Host "Error: Current results file not found: $CurrentFile" -ForegroundColor Red
    exit 1
}

Write-Host "Threshold: $ThresholdPercent%" -ForegroundColor Yellow
Write-Host ""

# Load JSON files
try {
    $baseline = Get-Content $BaselineFile | ConvertFrom-Json
    $current = Get-Content $CurrentFile | ConvertFrom-Json
}
catch {
    Write-Host "Error: Failed to parse JSON files: $_" -ForegroundColor Red
    exit 1
}

# Compare results
$results = Compare-BenchmarkResults -Baseline $baseline -Current $current -Threshold $ThresholdPercent

# Display results
if ($results.Regressions.Count -gt 0) {
    Write-Host "⚠️  Performance Regressions Detected ($($results.Regressions.Count))" -ForegroundColor Red
    Write-Host "============================================" -ForegroundColor Red
    Write-Host ""
    
    $results.Regressions | Format-Table -AutoSize -Property Benchmark, 
        @{Name='Baseline (ns)'; Expression={[math]::Round($_.BaselineMean, 2)}},
        @{Name='Current (ns)'; Expression={[math]::Round($_.CurrentMean, 2)}},
        @{Name='Change (%)'; Expression={$_.PercentChange}}
    
    Write-Host ""
}
else {
    Write-Host "✅ No performance regressions detected." -ForegroundColor Green
    Write-Host ""
}

if ($results.Improvements.Count -gt 0) {
    Write-Host "🚀 Performance Improvements Detected ($($results.Improvements.Count))" -ForegroundColor Green
    Write-Host "============================================" -ForegroundColor Green
    Write-Host ""
    
    $results.Improvements | Format-Table -AutoSize -Property Benchmark,
        @{Name='Baseline (ns)'; Expression={[math]::Round($_.BaselineMean, 2)}},
        @{Name='Current (ns)'; Expression={[math]::Round($_.CurrentMean, 2)}},
        @{Name='Change (%)'; Expression={$_.PercentChange}}
    
    Write-Host ""
}

# Exit with error code if regressions found
if ($results.Regressions.Count -gt 0) {
    Write-Host "Performance regression check failed. Please review the results above." -ForegroundColor Red
    exit 1
}
else {
    Write-Host "Performance regression check passed." -ForegroundColor Green
    exit 0
}
