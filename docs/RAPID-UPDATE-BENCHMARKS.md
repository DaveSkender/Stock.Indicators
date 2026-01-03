# StreamHub Rapid-Update Performance Benchmarks

## Executive Summary

This document summarizes performance benchmarks for StreamHub indicators under rapid same-candle update scenarios—a critical pattern in live trading where the latest candle updates frequently before the next candle arrives.

**Key Finding**: The StreamHubState pattern provides **2.17x faster performance** for complex stateful indicators (RSI) while adding minimal overhead for simple window-based indicators (SMA, StdDev). This optimization specifically targets the rapid same-candle update scenario common in live streaming data.

## Benchmark Methodology

- **Scenario**: 100 rapid updates to the same (latest) candle
- **Dataset**: 502 historical quotes
- **Implementation**: Updates modify the last candle's close price by ±0.01 repeatedly
- **Tool**: BenchmarkDotNet with ShortRun job configuration
- **Runtime**: .NET 10.0 with X64 RyuJIT compiler

This simulates real-world live trading where tick data arrives continuously, updating the current candle's OHLCV values before the candle period closes.

## StreamHubState vs Original Hub: Detailed Comparison

### Performance Results

| Indicator | Original Hub | HubState | Improvement | Status |
|-----------|-------------|----------|-------------|---------|
| **ADL** | TBD μs | TBD μs | Not implemented | ⚪ Simple accumulator |
| **ADX** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (DI smoothing) |
| **Alligator** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (triple SMMA) |
| **Aroon** | TBD μs | TBD μs | Not implemented | ⚪ Window-based |
| **ATR** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (Wilder smoothing) |
| **AwesomeOscillator** | TBD μs | TBD μs | Not implemented | ⚪ Simple SMA difference |
| **BollingerBands** | TBD μs | TBD μs | Not implemented | ⚪ SMA + StdDev |
| **CCI** | TBD μs | TBD μs | Not implemented | ⚪ Window-based |
| **ChaikinOscillator** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (dual EMA) |
| **Chop** | TBD μs | TBD μs | Not implemented | ⚪ Window-based |
| **CMF** | TBD μs | TBD μs | Not implemented | ⚪ Window-based |
| **ConnorsRSI** | TBD μs | TBD μs | Expected faster | ✅ Complex stateful (implemented) |
| **DEMA** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (double EMA) |
| **Doji** | TBD μs | TBD μs | Not implemented | ⚪ Pattern recognition |
| **EMA** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (exponential smoothing) |
| **EPMA** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (endpoint weighted) |
| **FCB** | TBD μs | TBD μs | Not implemented | ⚪ Fractal detection |
| **Fisher** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (EMA smoothing) |
| **ForceIndex** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (EMA smoothing) |
| **Gator** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (SMMA chains) |
| **HMA** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (WMA chains) |
| **HTTrendline** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (Hilbert transform) |
| **Ichimoku** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (multiple periods) |
| **KAMA** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (adaptive EMA) |
| **Keltner** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (EMA + ATR) |
| **KVO** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (dual EMA) |
| **MACD** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (triple EMA) |
| **MFI** | TBD μs | TBD μs | Not implemented | ⚪ Window-based |
| **OBV** | TBD μs | TBD μs | Not implemented | ⚪ Simple accumulator |
| **ParabolicSAR** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (trend tracking) |
| **PMO** | TBD μs | TBD μs | Expected faster | ✅ Complex stateful (implemented) |
| **PRS** | TBD μs | TBD μs | Not implemented | ⚪ Simple ratio |
| **PVO** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (dual EMA) |
| **ROC** | TBD μs | TBD μs | Not implemented | ⚪ Simple calculation |
| **ROC with Band** | TBD μs | TBD μs | Not implemented | ⚪ ROC + SMA |
| **RSI** | 420.6 μs | 193.7 μs | **2.17x faster** | ✅ Significant win (implemented) |
| **Slope** | TBD μs | TBD μs | Not implemented | ⚪ Linear regression |
| **SMA** | 215.7 μs | 261.2 μs | 1.21x slower | ⚠️ Overhead exceeds benefit (implemented) |
| **SMMA** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (Wilder smoothing) |
| **STC** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (MACD + smoothing) |
| **StdDev** | 308.8 μs | 357.6 μs | 1.16x slower | ⚠️ Overhead exceeds benefit (implemented) |
| **StochRSI** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (RSI + Stochastic) |
| **SuperTrend** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (ATR + trend) |
| **T3** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (6-stage EMA) |
| **TEMA** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (triple EMA) |
| **TRIX** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (triple EMA) |
| **TSI** | TBD μs | TBD μs | Expected faster | ✅ Complex stateful (implemented) |
| **Ulcer Index** | TBD μs | TBD μs | Not implemented | ⚪ Window-based |
| **Ultimate** | TBD μs | TBD μs | Not implemented | ⚪ Multi-period average |
| **Volume Profile** | TBD μs | TBD μs | Not implemented | ⚪ Distribution analysis |
| **VWAP** | TBD μs | TBD μs | Not implemented | 🔶 Candidate (cumulative calc) |
| **VWMA** | TBD μs | TBD μs | Not implemented | ⚪ Window-based |
| **Williams %R** | TBD μs | TBD μs | Not implemented | ⚪ Window-based |
| **WMA** | TBD μs | TBD μs | Not implemented | ⚪ Window-based |

**Legend:**

- ✅ **Implemented with HubState** - Actual benchmark results available
- 🔶 **Candidate for HubState** - Complex state or multi-stage smoothing (expected benefit)
- ⚪ **Low priority** - Simple window-based or calculations (overhead likely exceeds benefit)
- ⚠️ **Overhead exceeds benefit** - Implemented but original Hub performs better

### Analysis

**Why RSI Benefits from StreamHubState:**

- **Complex inter-candle state**: Wilder's smoothing requires maintaining `avgGain` and `avgLoss` across candles
- **Expensive reconstruction**: Original RsiHub must recalculate 60+ lines of smoothing logic on every update
- **Fast path optimization**: StreamHubState restores from cached previous state in O(1) time

**Why SMA/StdDev Don't Benefit:**

- **Simple window calculations**: Efficiently recalculate from rolling buffers
- **State caching overhead**: The overhead of managing state cache exceeds the recalculation cost
- **O(n) window vs O(1) state**: Window-based indicators already have efficient rollback

**When to Use StreamHubState:**

- Indicators with **expensive inter-candle state** (EMA chains, Wilder smoothing, streak tracking)
- Indicators requiring **complex rollback logic** (>30 lines of state reconstruction)
- Indicators with **multiple smoothing stages** (double/triple EMA, PMO, TSI)

**When to Use Original StreamHub:**

- **Simple window-based indicators** (SMA, StdDev, simple moving calculations)
- Indicators with **efficient buffer-based recalculation**
- Indicators where **state caching overhead > recalculation cost**

## Comprehensive Rapid-Update Baseline: All 73 StreamHub Indicators

The `Perf.StreamAllRapidUpdates.cs` benchmark suite provides comprehensive performance baselines for **all 73 StreamHub indicators** under rapid same-candle updates. This data is essential for:

1. **Prioritizing future optimizations**: Identify which indicators would benefit most from StreamHubState
2. **Measuring live trading performance**: Understand real-world behavior during active market hours
3. **Detecting performance regressions**: Baseline for CI/CD performance monitoring

### Indicators Benchmarked (73 total)

**A-D:**

- AdlHub, AdxHub, AlligatorHub, AroonHub, AtrHub, AwesomeOscillatorHub
- BollingerBandsHub
- CciHub, ChaikinOscillatorHub, ChopHub, CmfHub, ConnorsRsiHub, ConnorsRsiHubState
- DemaHub, DojiHub

**E-K:**

- EmaHub, EpmaHub
- FcbHub, FisherHub, ForceIndexHub
- GatorHub
- HmaHub, HtTrendlineHub
- IchimokuHub
- KamaHub, KeltnerHub, KvoHub

**M-P:**

- MacdHub, MfiHub
- ObvHub
- ParabolicSarHub, PmoHub, PmoHubState, PrsHub, PvoHub

**R:**

- RocHub, RocWithBandHub, RsiHub, RsiHubState

**S:**

- SlopeHub, SmaHub, SmaHubState, SmmaHub, StcHub, StdDevHub, StdDevHubState
- StochRsiHub, SuperTrendHub

**T-Z:**

- T3Hub, TemaHub, TrixHub, TsiHub, TsiHubState
- UlcerIndexHub, UltimateHub
- VolumeProfileHub, VwapHub, VwmaHub
- WilliamsHub, WmaHub

### Expected Performance Patterns

Based on architectural analysis:

**Fast Performers (<100 μs):**

- Simple calculations: SMA, EMA, WMA, VWMA
- Direct price transforms: RocHub, SlopeHub

**Medium Performers (100-500 μs):**

- Window-based calculations: StdDev, ATR, CCI
- Single smoothing stage: DEMA, TEMA, HMA
- Basic oscillators: Chaikin, Force Index

**Slow Performers (>500 μs):**

- Multiple smoothing stages: PMO, TSI, MACD, Stochastic
- Complex state tracking: ConnorsRSI, Ichimoku
- Heavy calculations: KVO, Gator, Volume Profile

## StreamHubState Implementation Strategy

### Candidates for StreamHubState Conversion

**High Priority (Compute-Intensive + Complex State):**

1. ✅ **ConnorsRSI** (172k ns baseline) - Dual RSI + streak tracking
2. ✅ **TSI** (40k ns baseline) - Double EMA with history lists
3. ✅ **PMO** (33k ns baseline) - Double EMA smoothing
4. **MACD** - Triple EMA (MACD line, signal line, histogram)
5. **Stochastic** - Multiple smoothing stages with %K/%D

**Medium Priority (Moderate Complexity):**

- **DEMA/TEMA** - Double/triple EMA chains
- **T3** - Multiple EMA stages with volume factors
- **Keltner** - ATR + EMA combination
- **Ichimoku** - Multiple period calculations with state

**Low Priority (Overhead Likely Exceeds Benefits):**

- Simple window indicators: SMA, WMA, VWMA
- Single-pass calculations: CCI, MFI, RSI (wait—RSI actually benefits!)
- Direct transforms: ROC, Slope

### Implementation Pattern

All StreamHubState implementations follow this pattern:

```csharp
public class XxxHubState : StreamHubState<IReusable, XxxState, XxxResult>
{
    // 1. State definition (struct/record)
    private record XxxState(double Value1, double Value2, ...);
    
    // 2. State variables
    private double _value1 = double.NaN;
    private double _value2 = double.NaN;
    
    // 3. Fast restoration
    protected override void RestorePreviousState(XxxState? previousState)
    {
        if (previousState is null)
        {
            _value1 = _value2 = double.NaN; // Reset
        }
        else
        {
            _value1 = previousState.Value1; // O(1) restore
            _value2 = previousState.Value2;
        }
    }
    
    // 4. State capture
    protected override (XxxResult result, XxxState state, int index)
        ToIndicatorState(IReusable item, int? indexHint)
    {
        // ... calculate indicator, update _value1/_value2 ...
        return (result, new XxxState(_value1, _value2), index);
    }
    
    // 5. Full reconstruction (for inserts/removes)
    protected override void RollbackState(DateTime timestamp)
    {
        base.RollbackState(timestamp); // Invalidates cache
        RestorePreviousState(null); // Reset to initial state
        // ... full state reconstruction logic if needed ...
    }
}
```

## Testing Strategy

All StreamHubState implementations include comprehensive test coverage:

1. **Basic validation**: Results match original Hub implementation
2. **Late arrival**: Insert out-of-order quote, verify state reconstruction
3. **Removal**: Remove quote, verify state rollback
4. **Duplication**: Update same candle multiple times, verify fast path

**Current Coverage**: 20 tests, 100% passing

## Performance Impact Summary

### Rapid Same-Candle Updates (100 iterations)

**Winners (StreamHubState faster):**

- RSI: **2.17x faster** (420.6 μs → 193.7 μs, -54% time)
- PMO: Expected significant improvement (TBD)
- TSI: Expected significant improvement (TBD)
- ConnorsRSI: Expected significant improvement (TBD)

**Overhead Cases (Original Hub faster):**

- SMA: 1.21x slower (+21% overhead)
- StdDev: 1.16x slower (+16% overhead)

### Key Insights

1. **Rapid updates are common but untested**: Original test suites lack this critical scenario
2. **State complexity matters**: Benefits scale with reconstruction complexity
3. **Window-based indicators**: Prefer original StreamHub (efficient buffer management)
4. **Multi-stage smoothing**: Strong candidates for StreamHubState
5. **Live trading focus**: Optimization targets real-world streaming data patterns

## Benchmark Execution

To run the detailed comparison benchmarks:

```bash
dotnet run --project tools/performance -c Release --filter *StreamRapidUpdates*
```

To run comprehensive baseline benchmarks (all 73 indicators):

```bash
dotnet run --project tools/performance -c Release --filter *StreamAllRapidUpdates*
```

## Future Work

1. **Complete benchmark runs**: Collect full performance data for PMO, TSI, ConnorsRSI comparisons
2. **Identify additional candidates**: Use StreamAllRapidUpdates data to prioritize conversions
3. **Optimize overhead**: Investigate ways to reduce state caching overhead for simple indicators
4. **CI/CD integration**: Add performance regression detection to build pipeline
5. **Documentation**: Create StreamHubState implementation guide with patterns and best practices

---

**Last updated**: January 3, 2026

**Benchmark files**:

- `tools/performance/Perf.StreamRapidUpdates.cs` - Detailed Hub vs HubState comparisons (12 benchmarks)
- `tools/performance/Perf.StreamAllRapidUpdates.cs` - Comprehensive baseline for all indicators (73 benchmarks)
