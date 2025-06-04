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
- **OHLC**: Open, High, Low, Close price data
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

## AI Assistant Guidelines

When suggesting code changes:
1. **Preserve existing patterns** - Follow established conventions
2. **Validate mathematical accuracy** - Financial calculations must be precise
3. **Include appropriate tests** - Both unit and edge case scenarios  
4. **Consider performance impact** - This library prioritizes efficiency
5. **Maintain backward compatibility** - Breaking changes require major version bump
6. **Add XML documentation** - All public APIs must be documented

## Getting Started for Contributors

1. Clone repository and restore packages: `dotnet restore`
2. Build solution: `dotnet build`
3. Run tests: `dotnet test`
4. Check performance: Navigate to `tests/performance` and run benchmarks

For new indicators, start with existing similar indicators as templates and follow the established patterns in the codebase.