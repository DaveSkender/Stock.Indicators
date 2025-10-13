# Deferred BufferList Implementations: T052 and T055

## Overview

Tasks T052 (VolatilityStop) and T055 (ZigZag) have been deferred due to fundamental incompatibilities between their calculation requirements and the streaming/incremental processing model used by BufferList implementations.

## Technical Analysis

### T052: VolatilityStop BufferList

**Issue**: Requires retroactive modifications to past results

The VolatilityStop indicator has a critical implementation detail that makes it unsuitable for streaming:

```csharp
// From VolatilityStop.StaticSeries.cs lines 119-132
// remove trend to first stop, since it is a guess
int cutIndex = results.FindIndex(x => x.IsStop ?? false);

for (int d = 0; d <= cutIndex; d++)
{
    VolatilityStopResult r = results[d];
    
    results[d] = r with {
        Sar = null,
        UpperBand = null,
        LowerBand = null,
        IsStop = null
    };
}
```

**Why this blocks BufferList implementation:**

1. The algorithm calculates an initial trend direction (long or short) as a "guess" before enough data is available
2. It processes all quotes using this guess until the first actual stop point is identified
3. After identifying the first stop, it goes back and **retroactively nulls out all results** before that point
4. This retroactive modification directly contradicts the streaming model where:
   - Each `Add()` call produces a final result
   - Past results in the list should not be modified by future data
   - Users expect results to be stable once emitted

**Potential workarounds considered:**

1. **Emit nulls until first stop**: Would require knowing future data to determine when to start emitting non-null values
2. **Emit preliminary values with flag**: Would violate the contract that BufferList results match Series results
3. **Buffer entire initial period**: Would require unbounded buffering until first stop is found (could be 100+ periods)

**Conclusion**: Deferred pending architectural discussion on handling indicators with retroactive adjustments.

### T055: ZigZag BufferList

**Issue**: Requires look-ahead and retroactive modifications

The ZigZag indicator identifies significant turning points (peaks and troughs) by looking ahead to confirm reversals. Key problems:

```csharp
// From ZigZag.StaticSeries.cs
// Algorithm requires scanning forward from a point to find:
// 1. Maximum high/low during a trend
// 2. Confirmation that trend has reversed by threshold %
// 3. Backfilling the line between confirmed points
```

**Why this blocks BufferList implementation:**

1. **Look-ahead requirement**: To confirm a peak at index i, the algorithm must scan forward to find when price moves sufficiently in the opposite direction
2. **Retroactive updates**: When a new extreme is found (higher high or lower low), the algorithm updates which point is the "true" turning point
3. **Line interpolation**: The ZigZag line connects confirmed points, requiring updates to all intermediate null results when a new point is confirmed
4. **Uncertain timing**: Cannot determine if current point is a ZigZag point until future data confirms the reversal

**Example scenario:**

```text
Period 100: High = 150 (candidate peak)
Period 101-110: Price stays near 150 (peak not confirmed yet)
Period 111: High = 155 (new candidate peak, Period 100 was NOT a peak)
Period 112-120: Price drops to 140 (confirms Period 111 as peak)

A streaming implementation would need to:
- Emit "peak at 100" after Period 110
- Retract that and emit "no peak at 100" at Period 111
- Update to "peak at 111" at Period 120
- Backfill line values for Periods 111-120
```

**Potential workarounds considered:**

1. **Delay emission**: Buffer results until confirmation (requires unbounded buffering)
2. **Preliminary signals**: Emit tentative points (violates parity with Series)
3. **Event-based updates**: Emit events when past results change (architectural change)

**Conclusion**: Deferred pending architectural discussion on handling indicators requiring look-ahead and retroactive adjustments.

## Recommendations

### Short-term

1. Document these indicators as "Series-only" in the catalog
2. Add validation in BufferList factory methods to throw `NotSupportedException` with clear explanation
3. Update indicator documentation to explain the limitation

### Long-term considerations

For indicators requiring retroactive adjustments, consider:

1. **Event-based streaming API**: Allow indicators to emit "correction" events for past results
2. **Windowed streaming**: BufferList variant that maintains a fixed-size window and allows modifications within that window
3. **Hybrid approach**: Stream preliminary results with a "finalized" flag after a confirmation period
4. **Separate indicator class**: Create "RealtimeVolatilityStop" with different behavior suitable for streaming

## Implementation Status

- **T052 (VolatilityStop)**: ✅ Implemented successfully (October 13, 2025)
  - Uses `UpdateInternal()` method added to BufferList base class
  - Retroactively nullifies results before first stop when it occurs
  - Maintains timestamp integrity (only updates Value properties, not Timestamp)
- **T053 (Vortex)**: ✅ Implemented successfully
- **T054 (Vwap)**: ✅ Implemented successfully
- **T055 (ZigZag)**: Marked as deferred in tasks.md

## Related Issues

This deferral affects similar indicators that may have look-ahead or retroactive requirements:

- Fractal (T021) - Already deferred, requires future data
- Ichimoku (T026) - Already deferred, requires future offsets
- Slope (T042) - Already deferred, retroactive Line property updates
- HtTrendline (T024) - Already deferred, complex Hilbert Transform
- Hurst (T025) - Already deferred, complex Hurst Exponent

---
Last updated: October 12, 2025
