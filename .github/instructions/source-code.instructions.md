---
applyTo: "src/**,tests/**"
description: ".NET development, testing, formatting, linting, and performance guidelines for Stock Indicators library"
---

# Source code and testing instructions

These instructions apply to all files in the `src/` and `tests/` folders and cover .NET development, unit testing, performance benchmarking, code formatting, and library-specific patterns.

## Build and development workflow

### Core development commands

```bash
# build the entire solution
dotnet build

# build in release mode
dotnet build -c Release

# restore NuGet packages
dotnet restore
```

### Code cleanup and formatting

```bash
# format all code using .editorconfig rules
dotnet format

# format with verification (non-destructive check)
dotnet format --verify-no-changes

# format specific projects
dotnet format src/Indicators.csproj
dotnet format tests/indicators/Tests.Indicators.csproj
```

## Testing guidelines

### Unit testing workflow

```bash
# run all unit tests
dotnet test

# run unit tests only (excludes integration tests)
dotnet test --settings tests/tests.unit.runsettings

# run integration tests only
dotnet test --settings tests/tests.integration.runsettings

# run with coverage reporting
dotnet test --collect:"Code Coverage"
```

### Test organization requirements

- **Unit tests**: Place in `tests/indicators/` following existing structure
- **Performance tests**: Use `tests/performance/` for benchmark tests
- **Integration tests**: Mark with `[TestCategory("Integration")]` attribute
- **External tests**: Place API tests in `tests/external/` folders

### Test patterns and standards

```csharp
[TestClass]
public class MyIndicatorTests : TestBase
{
    [TestMethod]
    public void StandardTest()
    {
        // Use existing test data patterns
        // Test against manually calculated results
        // Include edge cases and boundary conditions
    }
}
```

## Performance testing

### Benchmark execution

```bash
# run all performance benchmarks (~15-20 minutes)
cd tests/performance
dotnet run -c Release

# run specific benchmark
dotnet run -c Release --filter *.ToAdx

# run benchmark cohorts
dotnet run -c Release --filter *Stream*

# list available benchmarks
dotnet run --list
```

### Performance considerations

- Use `decimal` precision for financial calculations (never `double`)
- Minimize LINQ chaining in hot paths
- Implement efficient lookback period calculations
- Profile memory allocation patterns
- Test with realistic data volumes

## Code quality standards

### Required practices

- **Input validation**: Validate all public method parameters
- **XML documentation**: All public methods must have complete XML docs
- **Exception handling**: Use consistent error messages and exception types
- **Mathematical accuracy**: Test against proven reference implementations
- **Null safety**: Handle insufficient data scenarios gracefully

### Financial calculation guidelines

- Use `decimal` type for all price and monetary calculations
- Handle `double.NaN` appropriately in result types
- Implement `IReusable` interface correctly for chainable indicators
- Follow established patterns for lookback period validation
- Ensure calculation accuracy matches reference implementations

### Code organization patterns

```csharp
// Standard indicator structure
public static partial class Indicator
{
    public static IEnumerable<ResultType> ToIndicatorName<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
    {
        // Input validation
        // Core calculation logic
        // Return results
    }
}
```

## Library-specific requirements

### Indicator development checklist

- [ ] Implement series-style indicator method
- [ ] Create result model inheriting from `IReusable` or `ISeries`
- [ ] Add comprehensive unit tests with manual calculations
- [ ] Include performance benchmark if computationally intensive
- [ ] Create catalog entry following established patterns
- [ ] Update documentation in `docs/_indicators/`
- [ ] Validate mathematical accuracy against reference sources

### Common pitfalls to avoid

1. **Off-by-one errors** in lookback period calculations
2. **Null reference exceptions** with insufficient data
3. **Precision loss** using `double` instead of `decimal`
4. **Index out of bounds** when accessing historical data
5. **Performance regression** from excessive LINQ operations

### Result model patterns

```csharp
// IReusable pattern (single primary result)
public record EmaResult : IReusable
{
    public DateTime Timestamp { get; init; }
    public double? Ema { get; init; }
    public double Value => Ema.Null2NaN();
}

// ISeries pattern (multiple results)
public record MacdResult : ISeries
{
    public DateTime Timestamp { get; init; }
    public double? Macd { get; init; }
    public double? Signal { get; init; }
    public double? Histogram { get; init; }
}
```

## Linting and static analysis

### Code analysis tools

- **EditorConfig**: Follow formatting rules in `.editorconfig`
- **Roslyn analyzers**: Address all compiler warnings
- **Code coverage**: Maintain high test coverage (>95%)
- **Security scanning**: Use tools like Codacy for vulnerability detection

### Pre-commit validation

Before committing source code changes:

1. **Build verification**: `dotnet build` succeeds without warnings
2. **Format check**: `dotnet format --verify-no-changes` passes
3. **Unit tests**: All existing tests continue to pass
4. **New tests**: Add tests for any new functionality
5. **Performance check**: Run relevant benchmarks if performance-critical

### Continuous integration requirements

- All builds must complete without errors or warnings
- Unit test coverage must not decrease
- Performance benchmarks must not regress significantly
- Security scans must not introduce new vulnerabilities

---
Last updated: December 28, 2024
