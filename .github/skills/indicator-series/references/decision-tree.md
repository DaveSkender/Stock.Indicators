# Series indicator decision tree

Use this reference to guide Series indicator implementation decisions.

## When to create a new Series indicator

1. **New indicator algorithm**: Implementing a recognized technical analysis indicator
2. **Canonical reference**: Series provides the mathematical truth for other styles
3. **Batch processing**: Indicator processes complete datasets efficiently

## Input/output classification

| Input Type | Output Type | Interface | Examples |
| ---------- | ----------- | --------- | -------- |
| `IQuote` | Single chainable value | `IReusable` | EMA, SMA, RSI, ADX, MACD |
| `IQuote` | Multiple values (no single chainable) | `ISeries` | Alligator, Ichimoku, Keltner |
| `IReusable` | Single chainable value | `IReusable` | Chained indicators (EMA of RSI) |

**Note**: `IReusable` results have a defined chainable `Value` property. `ISeries` results have multiple outputs without a single chainable value. MACD implements `IReusable` with `Value => Macd.Null2NaN()`.

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
    // No single Value - cannot be chained
}
```

## Validation patterns

Use consistent exception handling:

- **Null inputs**: `ArgumentNullException.ThrowIfNull(quotes)`
- **Period ranges**: `ArgumentOutOfRangeException.ThrowIfLessThan(period, 1)`
- **Logical constraints**: `ArgumentException` for relationships (e.g., short period < long period)

## Performance considerations

| Scenario | Approach |
| -------- | -------- |
| Large datasets | Pre-allocate `List<T>(count)` or use arrays |
| Hot loops | Avoid LINQ, use `for` loops |
| Memory | Use `Span<T>` for slicing when beneficial |
| Precision | Use `double` internally, not `decimal` |

## Warmup period patterns

Warmup periods are determined by the original indicator formula specification:

| Pattern | Warmup Formula | Examples |
| ------- | -------------- | -------- |
| Simple average | `lookback - 1` | SMA |
| Exponential with seed | `lookback` | EMA |
| Wilder's smoothing | `2 * lookback - 1` | RSI, ATR, ADX |
| Multi-stage (EMA chain) | Sum of component warmups | MACD, TEMA |
| Compound indicator | Max of component warmups | Various |

---
Last updated: December 30, 2025
