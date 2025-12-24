# Test naming guidelines

This document defines conventions for contributors, **AI coding agents**, and automated tools
when creating or modifying tests in this repository.

<!-- ai:rule start -->
## Test method naming convention

### ✅ Required format

All test method names **must** follow this pattern:

```text
MethodName_StateUnderTest_ExpectedBehavior
```

Example:

```csharp
[TestMethod]
public void CalculateAlma_WhenInputIsNull_ReturnsNaN()
```

### Explanation

| Component          | Description                                                                    | Example           |
|--------------------|--------------------------------------------------------------------------------|-------------------|
| `MethodName`       | The production method or logical operation being tested. Use imperative verbs. | `CalculateAlma`   |
| `StateUnderTest`   | The specific condition, input, or setup for this test.                         | `WhenInputIsNull` |
| `ExpectedBehavior` | The expected outcome or system response.                                       | `ReturnsNaN`      |
<!-- ai:rule end -->

---

## Additional rules

<!-- ai:rule start -->
1. **Use underscores** (`_`) between each part for clarity. Never use spaces or hyphens. This is intentional and supported by analyzer suppression for tests (CA1707).

2. **Omit redundant context.** If the test class already specifies the system under test (e.g., `AlmaCalculatorTests`), you may drop the method name from the test:

   ```csharp
   [TestMethod]
   public void WhenInputIsNull_ReturnsNaN()
   ```

3. **Be explicit and deterministic.** Avoid generic phrases like `ValidInput` or `EdgeCase`. Use descriptive, repeatable states.

4. **Match behavior to assertions.** The `_ExpectedBehavior` portion should describe the result being asserted, not the action taken.

5. **Keep names short but complete.** Target under 80 characters. Split complex scenarios into multiple tests.

6. **Attributes and framework.** Preserve MSTest attributes: `[TestClass]` on classes and `[TestMethod]` on test methods.

7. **Namespaces and visibility.** Place test classes in appropriate namespaces mirroring source layout when feasible. Public visibility is acceptable for tests in this repo and is covered by existing suppressions.
<!-- ai:rule end -->

---

## Example test class

```csharp
[TestClass]
public class AlmaCalculatorTests
{
    [TestMethod]
    public void CalculateAlma_WhenInputIsNull_ReturnsNaN() { ... }

    [TestMethod]
    public void CalculateAlma_WithSingleQuote_ReturnsSameValue() { ... }

    [TestMethod]
    public void CalculateAlma_WithTypicalSeries_ReturnsSmoothedAverage() { ... }

    [TestMethod]
    public void CalculateAlma_WithShortWindow_ComputesCorrectly() { ... }
}
```

---

## Analyzer and tooling notes

The repository intentionally allows underscores in test identifiers to improve readability. The following configurations are in place:

- Directory-level suppression for all tests: `tests/Directory.Build.props` includes `<NoWarn>$(NoWarn);CA1707</NoWarn>`.
- Tests EditorConfig: `tests/.editorconfig` sets `dotnet_diagnostic.CA1707.severity = none` for C# files in tests.

These ensure that analyzer CA1707 does not block the required underscore naming convention in test method names while keeping production code unaffected.

## Test framework and assertion conventions

### Testing framework

- **MSTest**: Primary testing framework with `[TestClass]` and `[TestMethod]` attributes
- **FluentAssertions v6**: Modern assertion library for readable test assertions (pinned to v6 for licensing)
- **Test base classes**: **REQUIRED** - Inherit from appropriate base classes in `tests/indicators/_base/`

**Required base class inheritance:**

```csharp
// For series-style indicator tests
[TestClass]
public class MyIndicator : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard() { /* test implementation */ }
}

// For stream-style indicator tests  
[TestClass]
public class MyIndicatorStream : StreamHubTestBase
{
    [TestMethod]
    public override void Standard() { /* test implementation */ }
}

// For buffer-style indicator tests
[TestClass]
public class MyIndicatorList : BufferListTestBase
{
    [TestMethod]
    public override void Standard() { /* test implementation */ }
}

// For regression testing
[TestClass]
public class MyIndicatorRegression : RegressionTestBase
{
    [TestMethod]
    public void MyIndicator_WithStandardQuotes_MatchesBaseline() { /* test implementation */ }
}
```

### Assertion patterns

**✅ Required FluentAssertions syntax:**

```csharp
// Collections and counts
results.Should().HaveCount(502);
streamData.Should().BeEquivalentTo(staticData);

// Exception testing
Action act = () => quotes.ToIndicator(-1);
act.Should().Throw<ArgumentOutOfRangeException>()
   .WithMessage("*parameter*");

// Value comparisons (exact matches required)
result.Value.Should().Be(expected);
result.Adx.Should().Be(15.9459);

// Null checks
result.Value.Should().BeNull();
results.Should().NotBeNull();
```

### Mathematical precision requirements

**🎯 Core principle**: All mathematical indicators in this library are deterministic and repeatable.

**✅ Required precision standards:**

- Series-style indicators establish the mathematical truth
- Stream and Buffer implementations must match Series results **exactly**
- No approximation tolerances should be needed for properly implemented indicators
- Use exact equality comparisons: `result.Should().Be(expected)`

**🚫 Precision anti-patterns:**

```csharp
// NEVER use approximation for indicator results
result.Should().BeApproximately(expected, 0.0001);  // ❌

// NEVER use rounding to hide precision issues  
result.Round(4).Should().Be(expected.Round(4));     // ❌

// NEVER use tolerance for cross-implementation testing
streamResult.Should().BeApproximately(seriesResult, tolerance);  // ❌
```

#### 🔬 Exception: Series assertions against manually calculated reference values only

> STOP and ask if there are exceptional circumstances that require precision tolerance.

Use `BeApproximately()` **ONLY** in Series-style indicator tests when asserting against manually calculated reference values from authoritative sources (publications, verified calculations). This aligns with rounding in published references—it is not a workaround for floating-point errors in the indicator implementation or for loosening Stream/Buffer comparisons.

Use the FluentAssertions syntax with the subject on the left: `actual.Should().BeApproximately(expected, Money4);` (no `Round()` or ad-hoc tolerances).

> **Note**: `BeApproximately()` is specifically for floating-point types (`double`, `float`, `decimal`). `BeCloseTo()` is for integer types and should not be used for financial calculations.

**✅ Correct precision patterns:**

```csharp
// Exact comparisons for all indicator results (preferred)
result.Adx.Should().Be(15.9459);
streamResult.Should().BeEquivalentTo(seriesResult);

// Handle nulls explicitly without approximation
result.Value.Should().BeNull();
result.Sma.Should().Be(123.456789);

// ONLY for Series vs manually calculated reference values
result.Pdi.Should().BeApproximately(21.9669, Money4);  // Series vs manual calculation only
result.Mdi.Should().BeApproximately(18.5462, Money4);  // Series vs manual calculation only

// Handle nullable values with precision (Series vs manual only)
result.Adx?.Should().BeApproximately(15.9459, Money4);
```

**Precision tolerance mapping for BeApproximately():**

| Round() Digits | TestBase Constant | BeApproximately() Tolerance | Example                                 |
|----------------|-------------------|-----------------------------|-----------------------------------------|
| `.Round(3)`    | `Money3`          | `0.0005`                    | `BeApproximately(123.456, Money3)`      |
| `.Round(4)`    | `Money4`          | `0.00005`                   | `BeApproximately(15.9459, Money4)`      |
| `.Round(5)`    | `Money5`          | `0.000005`                  | `BeApproximately(1.23456, Money5)`      |
| `.Round(6)`    | `Money6`          | `0.0000005`                 | `BeApproximately(1.234567, Money6)`     |

> **Rationale**: Tolerance is half of the last decimal place to ensure only values that would round to the same result will pass.
> **Constants**: Use `Money3`, `Money4`, `Money5`, `Money6` from `TestBase` for consistent precision across all tests.

**🔄 Legacy patterns to be refactored:**

```csharp
// Legacy Round() usage - will be refactored to BeApproximately()
Assert.AreEqual(21.9669, r14.Pdi.Round(4));  // 🔄 Legacy - replace with BeApproximately(21.9669, Money4)
Assert.AreEqual(18.5462, r14.Mdi.Round(4));  // 🔄 Legacy - replace with BeApproximately(18.5462, Money4)
```

**🚫 Avoid these patterns:**

```csharp
// Do NOT use approximation - indicators are deterministic
result.Value.Should().BeApproximately(expected, precision);  // ❌

// Do NOT use legacy MSTest assertions - use FluentAssertions
Assert.AreEqual(expected, actual);  // ❌
Assert.IsNull(result.Value);        // ❌
```

**📝 Note:** FluentAssertions is pinned to v6 to avoid new licensing requirements.

### Test data conventions

**Test data sources:**

- **Static datasets**: Use `TestBase` properties like `Quotes`, `LongishQuotes`, `RandomQuotes`
- **CSV files**: Located in `tests/indicators/_testdata/quotes/` (e.g., `default.csv`, `bitcoin.csv`)
- **Baseline results**: JSON files in `tests/indicators/_testdata/results/` for regression testing
- **Custom data**: Use `Data.GetRandom()`, `Data.GetDefault()` methods from `TestData` classes

**File naming conventions:**

- **Quote data**: `{symbol}.csv` or `{description}.csv` in `_testdata/quotes/`
- **Expected results**: `{indicator}.{variant}.json` in `_testdata/results/`
- **Test files**: `{Indicator}.{TestType}.Tests.cs` (e.g., `Adx.StaticSeries.Tests.cs`)

**Data loading patterns:**

```csharp
// Use predefined datasets from TestBase
IReadOnlyList<Quote> quotes = Quotes;              // Standard dataset
IReadOnlyList<Quote> quotes = LongishQuotes;       // Longer dataset
IReadOnlyList<Quote> quotes = RandomQuotes;        // Random data

// Load custom CSV data
IReadOnlyList<Quote> quotes = Data.GetFile("bitcoin.csv");

// Load expected results for regression testing
IReadOnlyList<AdxResult> expected = Data.Results<AdxResult>("adx.default.json");
```

## Behavior for AI agents

<!-- ai:rule start -->
AI coding agents must:

1. **Naming**: Follow the `MethodName_StateUnderTest_ExpectedBehavior` format.
2. **Attributes**: Preserve `[TestMethod]` attributes for MSTest.
3. **Assertions**: Use FluentAssertions syntax exclusively (`Should()`) - avoid legacy MSTest assertions.
4. **Precision**: Use exact comparisons (`Should().Be()`) - indicators are deterministic. Only use `BeApproximately()` for Series assertions against manually calculated reference values.
5. **Precision constants**: Use `Money3`, `Money4`, `Money5`, `Money6` constants from `TestBase` for BeApproximately() tolerances.
6. **Base classes**: **REQUIRED** - Inherit from appropriate test base classes (`StaticSeriesTestBase`, `StreamHubTestBase`, `BufferListTestBase`, `RegressionTestBase`).
7. **Test data**: Use appropriate `TestBase` properties and `Data.GetFile()` methods for consistent test datasets.
8. **Deterministic results**: Stream and Buffer results must match Series results exactly without precision modifiers.
9. **Avoid BDD prefixes**: Do not add prefixes like `Should`, `Given`, `When`, or BDD-style variants unless explicitly allowed.
10. **Consistency**: Maintain naming consistency across refactors and new tests.
<!-- ai:rule end -->

---

## Summary

✅ **Use:**  
`CalculateAlma_WhenInputIsNull_ReturnsNaN()`

🚫 **Avoid:**  
`ShouldReturnNaN_WhenInputIsNull()`  
`GivenNullInput_WhenCalculatingAlma_ThenReturnsNaN()`

This ensures test output is consistent, readable, and compliant with automated analysis tools. The underscore pattern is intentional and supported by analyzer configuration in this repository.

---
Last updated: October 12, 2025
