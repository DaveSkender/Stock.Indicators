---
name: indicator-buffer
description: Implement BufferList incremental indicators with efficient state management. Use for IIncrementFromChain, IIncrementFromQuote, or IIncrementFromPairs implementations. Covers interface selection, constructor patterns, and BufferListTestBase testing requirements.
---

# BufferList indicator development

BufferList indicators process data incrementally with efficient buffering, matching Series results exactly.

## Interface selection

All BufferList implementations support `IQuote` inputs from the base class. The interface determines what *additional* input types are supported:

| Interface | Additional Inputs | Use Case | Examples |
| --------- | ----------------- | -------- | -------- |
| `IIncrementFromChain` | `IReusable`, `(DateTime, double)` | Chainable single-value indicators | SMA, EMA, RSI |
| `IIncrementFromQuote` | (none - only IQuote) | Requires OHLCV properties | Stoch, ATR, VWAP |
| `IIncrementFromPairs` | Dual `IReusable` | Two synchronized series | Correlation, Beta |

## Constructor pattern

```csharp
public class MyIndicatorList : BufferList<MyResult>, IIncrementFromChain
{
    private readonly Queue<double> _buffer;

    // Primary constructor (parameters only)
    public MyIndicatorList(int lookbackPeriods)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(lookbackPeriods, 1);
        LookbackPeriods = lookbackPeriods;
        _buffer = new Queue<double>(lookbackPeriods);
    }

    // Chaining constructor (parameters + quotes)
    public MyIndicatorList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);
}
```

## Buffer management

Use extension methods from `BufferListUtilities`:

- `_buffer.Update(capacity, value)` - Standard rolling buffer
- `_buffer.UpdateWithDequeue(capacity, value)` - Returns dequeued value for sum adjustment

> **Note**: Future refactor planned to rename `BufferListUtilities` to `BufferListExtensions` for .NET idiomatic naming.

## State management

Use `Clear()` to reset all internal state:

```csharp
public override void Clear()
{
    base.Clear();
    _buffer.Clear();
    _bufferSum = 0;
}
```

## Testing requirements

1. Inherit from `BufferListTestBase` (NOT TestBase)
2. Implement test interface matching buffer interface:
   - `ITestChainBufferList` for `IIncrementFromChain`
   - `ITestQuoteBufferList` for `IIncrementFromQuote`
   - `ITestPairsBufferList` for `IIncrementFromPairs`
3. Verify exact Series parity: `bufferResults.IsExactly(seriesResults)`

## Required test methods

All BufferList tests must pass these 5 base tests:

1. `AddQuote_IncrementsResults()`
2. `AddQuotesBatch_IncrementsResults()`
3. `QuotesCtor_OnInstantiation_IncrementsResults()`
4. `Clear_WithState_ResetsState()`
5. `PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()`

## Code completion checklist

- [ ] Source code: `src/**/{IndicatorName}.BufferList.cs` file exists
  - [ ] Inherits `BufferList<TResult>` and implements correct increment interface
  - [ ] Two constructors: primary + chaining via `: this(...) => Add(quotes);`
  - [ ] Uses `BufferListUtilities.Update()` or `UpdateWithDequeue()`
  - [ ] `Clear()` resets results and all internal buffers
- [ ] Unit testing: `tests/indicators/**/{IndicatorName}.BufferList.Tests.cs` exists
  - [ ] Inherits `BufferListTestBase` and implements correct test interface
  - [ ] All 5 required tests from base class pass
  - [ ] Verifies equivalence with Series results

## Anti-patterns to avoid

**Manual buffer management** (WRONG):

```csharp
if (_buffer.Count == capacity) _buffer.Dequeue();
_buffer.Enqueue(value);
```

**Use extension methods** (CORRECT):

```csharp
_buffer.Update(capacity, value);
```

## Reference implementations

- Chain: `src/e-k/Ema/Ema.BufferList.cs`
- Quote: `src/s-z/Stoch/Stoch.BufferList.cs`
- Pairs: `src/a-d/Correlation/Correlation.BufferList.cs`
- Complex: `src/a-d/Adx/Adx.BufferList.cs`

See `references/interface-selection.md` for detailed interface guidance.

---
Last updated: December 30, 2025
