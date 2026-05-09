# Memory Baseline Measurements (Q006)

This directory contains memory usage baselines for streaming indicators (BufferList and StreamHub implementations).

## Purpose

Track memory overhead for streaming indicators to ensure compliance with NFR-002 requirement:

- **Target**: <10KB memory overhead per StreamHub instance
- **Target**: <5KB memory overhead per BufferList instance

## Baseline Files

### Current Baselines

- `baseline-memory-v3.1.0.json` - Memory baselines for version 3.1.0
- `simple-indicators.json` - Memory patterns for simple indicators (SMA, EMA, ROC)
- `complex-indicators.json` - Memory patterns for complex indicators (ADX, MACD, Stochastic)
- `multi-series-indicators.json` - Memory patterns for multi-series indicators (Alligator, Bollinger Bands)
- `windowed-indicators.json` - Memory patterns for windowed indicators (SMA, WMA, HMA)

### Baseline Format

```json
{
  "version": "3.1.0",
  "date": "2025-12-27",
  "testEnvironment": {
    "runtime": ".NET 10",
    "os": "Ubuntu 22.04",
    "architecture": "x64"
  },
  "indicators": [
    {
      "name": "SmaHub",
      "category": "windowed",
      "lookbackPeriod": 14,
      "allocatedBytes": 24680,
      "instanceOverhead": 1856,
      "perPeriodCost": 45,
      "target": "<5KB",
      "status": "PASS"
    }
  ]
}
```

## Collecting Memory Baselines

### 1. Run benchmarks with memory diagnostics

```bash
cd tools/performance

# Memory diagnostics are automatically enabled in BenchmarkConfig
dotnet run -c Release
```

### 2. Extract memory data from results

```bash
# View memory allocation data
cat BenchmarkDotNet.Artifacts/results/Performance.StyleComparison-report-full.json | \
  jq '.Benchmarks[] | {
    Method: .Method,
    Mean: .Statistics.Mean,
    Allocated: .Memory.BytesAllocatedPerOperation,
    Gen0: .Memory.Gen0Collections,
    Gen1: .Memory.Gen1Collections,
    Gen2: .Memory.Gen2Collections
  }'
```

### 3. Calculate instance overhead

Instance overhead = Total allocated - (result storage + cache)

**Example for SmaHub:**

- Total allocated: 24,680 bytes
- Result storage: ~40 bytes × 502 periods = 20,080 bytes
- Cache overhead: ~8 bytes × 502 periods = 4,016 bytes
- Instance overhead: 24,680 - 20,080 - 4,016 = 584 bytes ✅

### 4. Categorize by indicator type

Group indicators by complexity and memory pattern:

**Simple Indicators** (<2KB overhead):

- Single buffer/state variable
- Examples: SMA, EMA, ROC, Momentum

**Complex Indicators** (2-5KB overhead):

- Multiple buffers/state variables
- Examples: ADX, MACD, Stochastic, RSI

**Multi-Series Indicators** (3-8KB overhead):

- Multiple output series with separate state
- Examples: Alligator, Bollinger Bands, Keltner Channels

**Windowed Indicators** (proportional to lookback):

- Memory scales with lookback period
- Formula: ~8 bytes × lookback + <1KB overhead
- Examples: SMA, WMA, HMA

## Memory Profiling Best Practices

### Measurement Environment

1. **Always use Release configuration** - Debug builds have higher overhead
2. **Run on consistent hardware** - Memory allocation patterns can vary
3. **Multiple iterations** - Account for GC variance
4. **Realistic data sizes** - Use 502 periods (standard test dataset)

### Analysis Guidelines

1. **Ignore one-time allocations** - Focus on per-operation costs
2. **Watch for unbounded growth** - Indicates memory leaks
3. **Consider GC pressure** - High Gen2 collections indicate issues
4. **Validate targets** - Ensure compliance with NFR-002

## Memory Compliance Validation

### Validation Script (Future Enhancement)

Create `detect-memory-regressions.ps1` to automate memory regression detection:

```powershell
# Compare memory allocations against baseline
# Flag indicators exceeding threshold
# Report memory leaks
# Validate <10KB target compliance
```

### Manual Validation

Extract memory data and check compliance:

```bash
# Check StreamHub compliance (<10KB overhead)
cat BenchmarkDotNet.Artifacts/results/Performance.StreamIndicators-report-full.json | \
  jq '.Benchmarks[] | select(.Memory.BytesAllocatedPerOperation > 10240) | 
    {method: .Method, allocated: .Memory.BytesAllocatedPerOperation}'

# Check BufferList compliance (<5KB overhead)
cat BenchmarkDotNet.Artifacts/results/Performance.BufferIndicators-report-full.json | \
  jq '.Benchmarks[] | select(.Memory.BytesAllocatedPerOperation > 5120) | 
    {method: .Method, allocated: .Memory.BytesAllocatedPerOperation}'
```

## Expected Memory Patterns

### Series Indicators (Baseline)

- Result collection: ~40 bytes per period
- For 502 periods: ~20KB total
- No persistent state

### BufferList Indicators

- Internal buffer: Depends on lookback period
  - 14-period SMA: ~112 bytes
  - 20-period EMA: ~160 bytes
- Expected overhead: **<5KB** beyond result storage
- **Target**: ✅ Should meet <5KB easily

### StreamHub Indicators

- Provider cache: ~8 bytes per period for references
- State variables:
  - Simple: <1KB
  - Complex: 2-5KB
  - Multi-series: <3KB
- Expected overhead: **<10KB** for most indicators
- **Target**: ✅ Should meet <10KB for properly implemented indicators

## Maintenance

### Update Schedule

- **Per release**: Create version-specific baseline
- **Monthly**: Review for regressions
- **Quarterly**: Archive old baselines

### Baseline Management

```bash
# Create new baseline after release
cp BenchmarkDotNet.Artifacts/results/Performance.StyleComparison-report-full.json \
   baselines/memory/baseline-memory-v3.1.0.json

# Extract memory-specific data
cat baselines/memory/baseline-memory-v3.1.0.json | \
  jq '{version, date, indicators: [.Benchmarks[] | {
    name: .Method,
    allocated: .Memory.BytesAllocatedPerOperation,
    gen0: .Memory.Gen0Collections,
    gen1: .Memory.Gen1Collections,
    gen2: .Memory.Gen2Collections
  }]}' > baselines/memory/processed-baseline.json
```

## Next Steps

1. ✅ Memory diagnostics enabled in BenchmarkConfig
2. ⏳ Run benchmarks to collect initial baseline data
3. ⏳ Analyze results and categorize by indicator type
4. ⏳ Create category-specific baseline files
5. ⏳ Document any indicators exceeding targets
6. ⏳ Create automated regression detection script

## References

- Main analysis document: `../../PERFORMANCE_ANALYSIS.md`
- Streaming plan: `../../../docs/plans/streaming-indicators.plan.md`
- NFR-002: <10KB memory overhead per StreamHub instance
- BenchmarkDotNet Memory Diagnoser: <https://benchmarkdotnet.org/articles/configs/diagnosers.html#memory-diagnoser>

---

Last updated: January 3, 2026
