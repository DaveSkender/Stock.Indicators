---
name: testing-standards
description: Testing conventions for Stock Indicators. Use for test naming (MethodName_StateUnderTest_ExpectedBehavior), FluentAssertions patterns, precision requirements, and test base class selection.
---

# Testing standards

## Test base class selection

| Style | Base Class | Purpose |
| ----- | ---------- | ------- |
| Series | `StaticSeriesTestBase` | Batch processing tests |
| Buffer | `BufferListTestBase` | Incremental processing tests |
| Stream | `StreamHubTestBase` | Real-time processing tests |
| Other | `TestBase` | General utility tests |

## Test naming convention

Pattern: `MethodName_StateUnderTest_ExpectedBehavior`

```csharp
[TestMethod]
public void ToEma_WithSmallDataset_CalculatesCorrectly() { }

[TestMethod]
public void ToRsi_WithInsufficientData_ReturnsEmpty() { }

[TestMethod]
public void AddQuote_WithNullInput_ThrowsArgumentNull() { }
```

## Required test coverage

MUST include these test types:

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

| Constant | Value | Use Case |
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

## Test data

Use standard test data from `Data.GetDefault()`:

- 502 quotes of historical data
- Consistent across all tests
- Excel reference file: `tests/_common/Data.Quotes.xlsx`

---
Last updated: December 31, 2025
