# Series indicator decision tree

Use this reference to guide Series indicator implementation decisions.

## When to create a new Series indicator

1. **New indicator algorithm**: Implementing a recognized technical analysis indicator
2. **Canonical reference**: Series provides the mathematical truth for other styles
3. **Batch processing**: Indicator processes complete datasets efficiently

## Input/output classification

| Input Type | Output Type | Interface | Examples |
| ---------- | ----------- | --------- | -------- |
| `IQuote` | Single value | `IReusable` | EMA, SMA, RSI |
| `IQuote` | Multi-value | `ISeries` | MACD, Alligator |
| `IReusable` | Single value | `IReusable` | Chained indicators |

## Result model selection

**Single chainable output** → Implement `IReusable`:

```csharp
public record EmaResult : IReusable
{
    public DateTime Timestamp { get; init; }
    public double? Ema { get; init; }
    public double Value => Ema.Null2NaN();
}
```

**Multiple outputs** → Implement `ISeries`:

```csharp
public record MacdResult : ISeries
{
    public DateTime Timestamp { get; init; }
    public double? Macd { get; init; }
    public double? Signal { get; init; }
    public double? Histogram { get; init; }
}
```

## Validation decision tree

```text
Is parameter a period/lookback?
├─ Yes → ArgumentOutOfRangeException.ThrowIfLessThan(value, minValue)
├─ Is parameter nullable and null?
│  └─ Check if required → throw ArgumentNullException
└─ Is parameter a logical constraint (e.g., short < long)?
   └─ Yes → throw ArgumentException with clear message
```

## Performance considerations

| Scenario | Approach |
| -------- | -------- |
| Large datasets | Pre-allocate `List<T>(count)` |
| Hot loops | Avoid LINQ, use `for` loops |
| Memory | Use `Span<T>` for slicing |
| Precision | Use `double` internally, not `decimal` |

## Common warmup period patterns

| Pattern | Warmup Formula |
| ------- | -------------- |
| Simple average | `lookback - 1` |
| Exponential with seed | `lookback` |
| Wilder's smoothing | `2 * lookback - 1` (recommended) |
| Multi-stage (EMA chain) | Sum of all component warmups |
| Compound indicator | Max of component warmups |

---
Last updated: December 30, 2025
