# PairsProvider Re-implementation Plan

**Status**: Deferred  
**Author**: Copilot Agent  
**Date**: December 31, 2025  
**Related**: Streaming Indicators Feature

---

## Executive Summary

The PairsProvider pattern for dual-stream indicators (Beta, Correlation, PRS) has been **removed** from the StreamHub and BufferList implementations due to fundamental synchronization challenges. This document captures the requirements, design challenges, and guidance for potential future re-implementation.

**Static Series implementations remain intact** - only the streaming/incremental variants were removed.

---

## Affected Indicators

The following indicators require dual-input (pairs) processing:

| Indicator | Description | Series Status | Streaming Status |
| --------- | ----------- | ------------- | ---------------- |
| **Beta** | Beta coefficient comparing asset to market | ✅ Retained | ❌ Removed |
| **Correlation** | Pearson correlation coefficient | ✅ Retained | ❌ Removed |
| **PRS** | Price Relative Strength ratio | ✅ Retained | ❌ Removed |

---

## Key Design Challenge

The fundamental problem identified during testing (see PRS StreamHub tests) involves **synchronization of dual-stream mutations**.

### The Core Issue

From `tests/indicators/m-r/Prs/Prs.StreamHub.Tests.cs`:

```csharp
// TODO: test and handle matching removals in paired observers
// Removing from one side should auto-remove from both sides; however, there's a challenging synchronization issue here overall.
// If there's a "fix" situation, where one side is then restored we'd need to remember and restore.
// Rules we really need:
// 1. Adding a quote to one side is "staged" but not finalized until matching quote is added to other side.
// 2. Removing a quote from one side automatically re-"stages" from other side (waiting)
// 3. It's likely okay to "skip" staged items and continue with the next matching pairs, which aligns with removing an old one that's now staged.
```

### Problem Breakdown

1. **Insert Synchronization**: When a late quote arrives on provider A, it must be paired with the corresponding quote on provider B. If provider B doesn't have that timestamp yet, the result cannot be calculated.

2. **Remove Synchronization**: When a quote is removed from provider A, the corresponding calculation must be invalidated. But what if provider B still has its quote? The pair is broken.

3. **State Restoration**: If a removed quote is later restored (via Insert), the paired calculation must be rebuilt. This requires tracking "staged" or "pending" states for unpaired quotes.

4. **Rollback Complexity**: The existing `RollbackState(DateTime timestamp)` pattern assumes single-stream operations. Dual-stream rollback requires coordinating state across two independent caches.

---

## Previous Implementation Architecture

### PairsProvider Base Class

```csharp
public abstract class PairsProvider<TIn, TOut>(
    IStreamObservable<TIn> providerA,
    IStreamObservable<TIn> providerB
) : StreamHub<TIn, TOut>(providerA), IChainProvider<TOut>, IPairsObserver<TIn>
     where TIn : IReusable
     where TOut : IReusable
{
    protected IReadOnlyList<TIn> ProviderCacheB { get; }
    
    protected void ValidateTimestampSync(int index, TIn currentItem);
    protected bool HasSufficientData(int index, int minimumPeriods);
}
```

### IIncrementFromPairs Interface

```csharp
public interface IIncrementFromPairs
{
    void Add(DateTime timestamp, double valueA, double valueB);
    void Add(IReusable valueA, IReusable valueB);
    void Add(IReadOnlyList<IReusable> valuesA, IReadOnlyList<IReusable> valuesB);
}
```

### Test Interfaces

```csharp
public interface ITestPairsObserver
{
    void PairsObserver_SynchronizedProviders_MatchesSeriesExactly();
    void PairsObserver_TimestampMismatch_ThrowsInvalidQuotesException();
    void PairsObserver_WithSameProvider_HasFlatlineResults();
}

public interface ITestPairsBufferList
{
    void AddReusablePair_IncrementsResults();
    void AddReusablePairBatch_IncrementsResults();
    void AddDiscretePairs_IncrementsResults();
}
```

---

## Requirements for Re-implementation

### Functional Requirements

1. **Timestamp Synchronization**: Both input streams must have matching timestamps for paired calculations.

2. **Staged Addition Pattern**:
   - When a quote arrives on stream A, check if matching timestamp exists on stream B
   - If match exists: calculate immediately
   - If no match: stage the quote as "pending"
   - When matching quote arrives on stream B: calculate using staged + new

3. **Coordinated Removal Pattern**:
   - When a quote is removed from stream A, mark the corresponding result as invalid
   - Stage the orphaned stream B quote as "pending" (awaiting re-pairing)
   - Rebuild downstream results

4. **Rollback Coordination**:
   - `RollbackState()` must handle dual-cache state restoration
   - Consider introducing a `PairsRollbackState()` or similar pattern

5. **Error Handling**:
   - Clear error when timestamps permanently mismatch
   - Timeout or cleanup for long-pending staged quotes

### Non-Functional Requirements

1. **Performance**: Staged quote tracking should not introduce O(n²) complexity
2. **Memory**: Pending queue should be bounded (suggest: 2x lookback period)
3. **Thread Safety**: Consider concurrent adds to both streams

---

## Proposed Architecture

### Option 1: Paired Queue Coordinator

Introduce a coordinating layer that buffers incoming quotes until pairs are complete:

```csharp
public class PairedQuoteCoordinator<T> where T : IReusable
{
    private readonly Dictionary<DateTime, T> _pendingA = new();
    private readonly Dictionary<DateTime, T> _pendingB = new();
    
    public event Action<T, T>? OnPairComplete;
    public event Action<DateTime>? OnPairBroken;
    
    public void AddA(T item);
    public void AddB(T item);
    public void RemoveA(DateTime timestamp);
    public void RemoveB(DateTime timestamp);
}
```

### Option 2: Transaction-Based Mutations

Require paired operations to be atomic:

```csharp
public interface IPairsTransaction
{
    void BeginTransaction();
    void AddPair(IReusable a, IReusable b);
    void RemovePair(DateTime timestamp);
    void CommitTransaction();
    void RollbackTransaction();
}
```

### Option 3: Eventual Consistency Model

Allow temporary inconsistency with automatic reconciliation:

```csharp
public class EventualPairsProvider<TIn, TOut> : PairsProvider<TIn, TOut>
{
    private readonly TimeSpan _reconciliationWindow = TimeSpan.FromSeconds(5);
    
    protected override void OnQuoteAdded(TIn item)
    {
        if (!HasMatchingPair(item.Timestamp))
        {
            ScheduleReconciliation(item.Timestamp);
            return; // Skip calculation for now
        }
        
        base.OnQuoteAdded(item);
    }
}
```

---

## Implementation Roadmap

### Phase 1: Design Validation

1. Create proof-of-concept with Option 1 (Paired Queue Coordinator)
2. Validate against Insert/Remove test scenarios
3. Measure performance overhead

### Phase 2: Framework Integration

1. Extend `StreamHub` base class with optional pairs support
2. Implement `PairsRollbackState()` pattern
3. Update `Rebuild()` to handle dual-cache coordination

### Phase 3: Indicator Re-implementation

1. Re-implement CorrelationHub with new architecture
2. Re-implement BetaHub
3. Re-implement PrsHub
4. Restore BufferList variants

### Phase 4: Testing and Documentation

1. Comprehensive Insert/Remove/Rollback testing
2. Performance benchmarking vs Series
3. Documentation updates

---

## Code Samples (Removed Implementation)

### CorrelationHub (Removed)

```csharp
public class CorrelationHub : PairsProvider<IReusable, CorrResult>, ICorrelation
{
    protected override (CorrResult result, int index) ToIndicator(IReusable item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);
        
        if (HasSufficientData(i, LookbackPeriods))
        {
            ValidateTimestampSync(i, item);
            
            double[] dataA = new double[LookbackPeriods];
            double[] dataB = new double[LookbackPeriods];
            
            for (int p = 0; p < LookbackPeriods; p++)
            {
                int index = i - LookbackPeriods + 1 + p;
                dataA[p] = ProviderCache[index].Value;
                dataB[p] = ProviderCacheB[index].Value;
            }
            
            return (Correlation.PeriodCorrelation(item.Timestamp, dataA, dataB), i);
        }
        
        return (new CorrResult(Timestamp: item.Timestamp), i);
    }
}
```

### BetaHub Rolling Window State (Removed)

```csharp
private sealed class RollingWindowState(int capacity)
{
    public double[] WindowEval = new double[capacity];
    public double[] WindowMrkt = new double[capacity];
    public int WindowIndex;
    public int WindowCount;
    public double SumEval;
    public double SumMrkt;
    public double SumEval2;
    public double SumMrkt2;
    public double SumCross;
}
```

### CorrelationList (Removed)

```csharp
public class CorrelationList : BufferList<CorrResult>, IIncrementFromPairs, ICorrelation
{
    private readonly Queue<(double ValueA, double ValueB)> _buffer;
    
    public void Add(DateTime timestamp, double valueA, double valueB)
    {
        _buffer.Update(LookbackPeriods, (valueA, valueB));
        
        if (_buffer.Count == LookbackPeriods)
        {
            // Calculate correlation from buffer
        }
    }
}
```

---

## Files Removed

### Source Files

- `src/_common/StreamHub/Providers/PairsProvider.cs`
- `src/_common/StreamHub/IPairsObserver.cs`
- `src/_common/BufferLists/IIncrementFromPairs.cs`
- `src/a-d/Beta/Beta.StreamHub.cs`
- `src/a-d/Beta/Beta.BufferList.cs`
- `src/a-d/Correlation/Correlation.StreamHub.cs`
- `src/a-d/Correlation/Correlation.BufferList.cs`
- `src/m-r/Prs/Prs.StreamHub.cs`
- `src/m-r/Prs/Prs.BufferList.cs`

### Test Files

- `tests/indicators/a-d/Beta/Beta.StreamHub.Tests.cs`
- `tests/indicators/a-d/Beta/Beta.BufferList.Tests.cs`
- `tests/indicators/a-d/Correlation/Correlation.StreamHub.Tests.cs`
- `tests/indicators/a-d/Correlation/Correlation.BufferList.Tests.cs`
- `tests/indicators/m-r/Prs/Prs.StreamHub.Tests.cs`
- `tests/indicators/m-r/Prs/Prs.BufferList.Tests.cs`

---

## References

- [Streaming Indicators Plan](streaming-indicators.plan.md)
- [Beta Series Implementation](../../src/a-d/Beta/Beta.StaticSeries.cs)
- [Correlation Series Implementation](../../src/a-d/Correlation/Correlation.StaticSeries.cs)
- [PRS Series Implementation](../../src/m-r/Prs/Prs.StaticSeries.cs)

---
Last updated: December 31, 2025
