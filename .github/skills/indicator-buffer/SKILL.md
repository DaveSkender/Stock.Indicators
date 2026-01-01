---
name: indicator-buffer
description: Implement BufferList incremental indicators with efficient state management. Use for IIncrementFromChain or IIncrementFromQuote implementations. Covers interface selection, constructor patterns, and BufferListTestBase testing requirements.
---

# BufferList indicator development

## Interface selection

All BufferList implementations support `IQuote` inputs from the base class. The interface determines what *additional* input types are supported:

| Interface | Additional Inputs | Use Case | Examples |
| --------- | ----------------- | -------- | -------- |
| `IIncrementFromChain` | `IReusable`, `(DateTime, double)` | Chainable single-value indicators | SMA, EMA, RSI |
| `IIncrementFromQuote` | (none - only IQuote) | Requires OHLCV properties | Stoch, ATR, VWAP |

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

## Testing constraints

- Inherit `BufferListTestBase` (NOT `TestBase`)
- Implement test interface matching increment interface:
  - `IIncrementFromChain` → `ITestChainBufferList`
  - `IIncrementFromQuote` → `ITestQuoteBufferList`
- Verify exact Series parity with `bufferResults.IsExactly(seriesResults)` (NOT `Should().Be()`)
- All 5 base class tests pass (incremental adds, batching, constructor chaining, Clear(), auto-pruning)

## Required implementation

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

## Examples

- Chain: `src/e-k/Ema/Ema.BufferList.cs`
- Quote: `src/s-z/Stoch/Stoch.BufferList.cs`
- Complex: `src/a-d/Adx/Adx.BufferList.cs`

See `references/interface-selection.md` for interface decision guidance.

---
Last updated: December 31, 2025
