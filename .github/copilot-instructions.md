# GitHub Copilot Instructions for Stock Indicators for .NET

## Repository Overview

This is **Stock Indicators for .NET** - a comprehensive C# library providing 200+ technical analysis indicators for financial data analysis. The library focuses on performance, accuracy, and ease of use for .NET developers working with financial data.

## Project Structure

```
src/
├── _common/           # Shared utilities, base classes, and common types
├── a-d/              # Indicators A-D (alphabetical organization)
├── e-k/              # Indicators E-K
├── m-r/              # Indicators M-R
├── s-z/              # Indicators S-Z
└── Indicators.csproj # Main project file

tests/
├── indicators/       # Unit tests for indicators
├── other/           # Integration and utility tests
└── performance/     # Performance benchmarks
```

## Coding Standards & Conventions

### Naming Conventions
- **Classes**: PascalCase (e.g., `BollingerBands`, `RelativeStrengthIndex`)
- **Methods**: PascalCase (e.g., `GetBollingerBands()`, `GetRsi()`)
- **Properties**: PascalCase (e.g., `Upper`, `Lower`, `Signal`)
- **Parameters**: camelCase (e.g., `lookbackPeriods`, `percentK`)
- **Private fields**: camelCase with underscore prefix (e.g., `_quotes`, `_periods`)

### File Organization
- Each indicator has its own folder containing:
  - `{IndicatorName}.cs` - Main calculation logic
  - `{IndicatorName}.Models.cs` - Result models and configuration
  - `{IndicatorName}.Validation.cs` - Input validation (if complex)

### Code Patterns

#### Indicator Method Signature
```csharp
public static IEnumerable<IndicatorResult> GetIndicator<T>(
    this IEnumerable<T> quotes,
    int lookbackPeriods = defaultValue) where T : IQuote
```

#### Result Model Pattern
```csharp
public sealed class IndicatorResult : ResultBase
{
    public decimal? Value { get; set; }
    public decimal? Signal { get; set; }
    // Additional indicator-specific properties
}
```

#### Validation Pattern
```csharp
// Always validate inputs
quotes.ValidateQuotes();
if (lookbackPeriods <= 0)
    throw new ArgumentOutOfRangeException(nameof(lookbackPeriods));
```

## Testing Standards

### Unit Test Structure
- Located in `tests/indicators/{indicator-name}/`
- Test file naming: `Test.{IndicatorName}.cs`
- Use descriptive test method names: `GetIndicator_WithValidData_ReturnsExpectedResults()`

### Test Data
- Use consistent historical data from `TestData.GetDefault()`
- Include edge cases: insufficient data, boundary values
- Verify precision to appropriate decimal places

### Performance Tests
- Located in `tests/performance/`
- Benchmark critical calculation paths
- Monitor memory allocation patterns

## Financial Domain Knowledge

### Key Concepts
- **OHLCV**: Open, High, Low, Close, Volume price data
- **Lookback Period**: Number of periods required for calculation
- **Signal Line**: Secondary indicator line (often smoothed)
- **Convergence/Divergence**: Relationship between price and indicator

### Common Indicator Categories
- **Trend**: Moving averages, MACD, ADX
- **Momentum**: RSI, Stochastic, Williams %R
- **Volatility**: Bollinger Bands, ATR, Standard Deviation
- **Volume**: OBV, Accumulation/Distribution, Money Flow Index

## Performance Considerations

### Optimization Guidelines
- Use `Span<T>` and `ReadOnlySpan<T>` for array operations
- Minimize object allocations in tight loops
- Prefer LINQ for readability, manual loops for performance-critical paths
- Cache expensive calculations when possible

### Memory Management
- Return `IEnumerable<T>` for lazy evaluation
- Use `yield return` for streaming results
- Dispose of resources properly in using statements

## Common Pitfalls to Avoid

1. **Off-by-one errors** in lookback period calculations
2. **Null reference exceptions** with insufficient data
3. **Precision loss** in financial calculations - use `decimal` not `double`
4. **Index out of bounds** when accessing historical data
5. **Performance regression** from excessive LINQ chaining

## Dependencies & Libraries

### Primary Dependencies
- **.NET Standard 2.0** - For broad compatibility
- **System.Linq** - For data manipulation
- **No external math libraries** - All calculations are native

### Test Dependencies
- **Microsoft.NET.Test.Sdk**
- **MSTest.TestAdapter** and **MSTest.TestFramework**
- **BenchmarkDotNet** - For performance testing

## Code Review Guidelines

### What to Look For
- Input validation completeness
- Edge case handling (insufficient data, zero/negative values)
- Mathematical accuracy vs reference implementations
- Performance characteristics
- XML documentation completeness
- Consistent error messages and exception types

### Code Quality Standards
- All public methods must have XML documentation
- Unit test coverage for all code paths
- Performance tests for computationally intensive indicators
- Validation for all user inputs
- Consistent formatting using `.editorconfig`

## 🤖 AI Assistant Guidelines

When suggesting code changes:
1. **Preserve existing patterns** – Always check similar indicators before introducing new approaches.
2. **Validate mathematical accuracy** – Financial calculations must be precise and use `decimal`.
3. **Include comprehensive tests** – Cover both standard and edge cases, and verify precision.
4. **Consider performance impact** – Use memory-efficient patterns (`yield return`, `Span<T>`, avoid unnecessary allocations).
5. **Maintain backward compatibility** – Breaking changes require justification and a major version bump.
6. **Add XML documentation** – All public APIs must be documented with `<include file=.../>`.
7. **Document validation and error handling** – Ensure all user inputs are validated and errors are clear.

### PR Title Guidelines (Required by CI)
Your PR title **must** follow this format:
```
type: Subject with uppercase first letter
```
Where `type` is one of:
- feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert (all lowercase)
and `Subject` starts with an uppercase letter.

**Examples:**
```
feat: Add Kaufman Adaptive Moving Average indicator
fix: Correct RSI calculation for edge cases
docs: Update XML documentation for MACD
perf: Optimize Bollinger Bands calculation
test: Add unit tests for Stochastic indicator
```

**Invalid:**
```
feat: add new indicator        (lowercase subject)
Add new indicator              (missing type)
feat:Add new indicator         (missing space after colon)
FEAT: Add new indicator        (uppercase type)
```

### Example AI Response Pattern
```
I'll help you implement the [Indicator Name]. Based on the existing patterns in this codebase:

1. **File Structure**: Create these files in `src/[a-z]/IndicatorName/`:
   - `IndicatorName.cs` - Main calculation
   - `IndicatorName.Models.cs` - Result model

2. **Implementation**: Following the EMA pattern:
   ```csharp
   // Include specific code following repo patterns
   ```

3. **Tests**: Add to `tests/indicators/[a-z]/IndicatorName/`:
   ```csharp
   // Include test examples
   ```

4. **Performance Considerations**:
   - Use `yield return` for streaming
   - Validate inputs once at the start
   - Consider caching for repeated calculations
```
