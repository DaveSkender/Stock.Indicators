# Testing patterns

## Required test coverage

MUST include these test types for every indicator.

### Standard tests

```csharp
[TestMethod]
public void Standard()
{
    // Test against historical data with expected results
    var results = quotes.ToIndicator(14);

    // Verify specific data points
    var result = results.ElementAt(index);
    result.Value.Should().BeApproximately(expected, precision);
}
```

### Boundary tests

```csharp
[TestMethod]
public void MinimumPeriods()
{
    // Test with minimum lookback
    var results = quotes.ToIndicator(2);
    results.Should().NotBeEmpty();
}
```

### Bad data tests

```csharp
[TestMethod]
public void BadParameter()
{
    Action act = () => quotes.ToIndicator(-1);
    act.Should().Throw<ArgumentOutOfRangeException>();
}

[TestMethod]
public void NullQuotes()
{
    Action act = () => ((IReadOnlyList<Quote>)null!).ToIndicator(14);
    act.Should().Throw<ArgumentNullException>();
}
```

### Insufficient data tests

```csharp
[TestMethod]
public void InsufficientQuotes()
{
    var results = quotes.Take(5).ToList().ToIndicator(20);
    results.Should().BeEmpty();
}
```

## FluentAssertions patterns

```csharp
// Exact equality
result.Value.Should().Be(expected);

// Approximate equality with precision
result.Value.Should().BeApproximately(expected, 0.0001);

// Null checks
result.Value.Should().BeNull();
result.Value.Should().NotBeNull();

// Collection assertions
results.Should().HaveCount(502);
results.Should().BeEmpty();
results.Should().NotBeEmpty();

// Exception assertions
act.Should().Throw<ArgumentOutOfRangeException>();
act.Should().Throw<ArgumentNullException>()
   .WithParameterName("quotes");
```

## Precision constants

Use constants from `TestBase`:

| Constant | Value | Use case |
| -------- | ----- | -------- |
| `Money6` | 0.000001 | Most calculations |
| `Money5` | 0.00001 | Lower precision |
| `Money4` | 0.0001 | Standard tolerance |
| `Percent` | 0.01 | Percentage values |

```csharp
result.Ema.Should().BeApproximately(123.456789, Money6);
```

## Series parity validation

Buffer and Stream tests must validate against Series:

```csharp
[TestMethod]
public void MatchesSeries()
{
    // Get Series results (canonical reference)
    var series = quotes.ToIndicator(14);

    // Get Buffer results
    var buffer = new IndicatorList(14) { quotes };

    // Validate exact parity
    buffer.IsExactly(series);
}
```

## BufferList constraints

MUST implement these 5 base class tests:

1. `AddQuote_IncrementsResults()`
2. `AddQuotesBatch_IncrementsResults()`
3. `QuotesCtor_OnInstantiation_IncrementsResults()`
4. `Clear_WithState_ResetsState()`
5. `PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()`

## StreamHub constraints

All StreamHub tests must validate rollback scenarios:

- Prefill warmup window before subscribing
- Stream in-order including duplicates
- Insert late historical quote → verify recalculation
- Remove historical quote → verify recalculation
- Compare to Series with strict ordering
