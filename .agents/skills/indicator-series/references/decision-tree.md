# Result interface selection

**Single chainable output** → Implement `IReusable`:

```csharp
public record EmaResult
(
    DateTime Timestamp,
    double? Ema
) : IReusable
{
    [JsonIgnore]
    public double Value => Ema.Null2NaN();  // Enables chaining
}
```

**Multiple outputs without natural chainable value** → Implement `ISeries`:

```csharp
public record AlligatorResult
(
    DateTime Timestamp,
    double? Jaw,
    double? Teeth,
    double? Lips
) : ISeries;
```

**Multiple outputs WITH a primary chainable value** → Implement `IReusable`:

```csharp
public record MacdResult
(
    DateTime Timestamp,
    double? Macd,
    double? Signal,
    double? Histogram
) : IReusable
{
    [JsonIgnore]
    public double Value => Macd.Null2NaN();  // MACD line is the primary value
}
```

## Input type patterns

| Input Type | Use Case | Examples |
| ---------- | -------- | -------- |
| `IQuote` | Requires OHLCV data | ATR, ADX, Stochastic, Chandelier |
| `IReusable` | Chains from another indicator | EMA-of-EMA, SMA-of-RSI |

Most indicators accept `IQuote`. Use `IReusable` only for indicators explicitly designed to chain.

---
Last updated: December 31, 2025
