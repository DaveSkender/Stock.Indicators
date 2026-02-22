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

## Required test types

MUST include for every indicator:

- `Standard` — spot checks against historical reference values
- `MinimumPeriods` — boundary case with minimum lookback
- `BadParameter` — `ArgumentOutOfRangeException` for invalid inputs
- `NullQuotes` — `ArgumentNullException` for null inputs
- `InsufficientQuotes` — empty result when data < warmup period
- `MatchesSeries` — Buffer/Stream exact parity against Series

See [references/patterns.md](references/patterns.md) for code examples of each test type, FluentAssertions patterns, precision constants, and BufferList/StreamHub constraints.

## Test data

Use standard test data from `Data.GetDefault()`:

- 502 quotes of historical data
- Consistent across all tests
- Excel reference file: `tests/_common/Data.Quotes.xlsx`
