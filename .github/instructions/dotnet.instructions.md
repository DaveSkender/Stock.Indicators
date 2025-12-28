---
applyTo: "src/**"
description: ".NET development and coding standards"
---

# .NET development instructions

These instructions apply to all C# source code in the `src/` folder and cover coding standards, project organization, and implementation best practices. For broader repository guidance, custom agents, and architectural principles, see the [main Copilot instructions](../copilot-instructions.md).

## Code style and formatting

### C# language features

- Use modern C# features (target language version matches the framework)
- Use `nullable` reference types throughout
- Apply `required` keyword for mandatory initialization
- Use pattern matching and switch expressions where appropriate
- Use records for immutable data structures when suitable

### Code formatting

- Follow the `.editorconfig` configuration for consistent formatting (4-space indentation for C#, 2-space for other files)
- Run `dotnet format` before committing code
- **Prefer explicit types over `var`** for clarity and consistency (following `.editorconfig` conventions)
- Prefer expression bodies for simple methods and properties
- Use `#pragma` directives sparingly for suppressing warnings; document justifications
- Indent 4 spaces per level in C# code; use LF line endings (enforced by `.editorconfig`)

For detailed style guidance, consult [.editorconfig](./.editorconfig) and official C# coding conventions via #tool:mslearn.

### Naming conventions

- Use `PascalCase` for public types, methods, and properties
- Use `camelCase` for local variables and parameters
- Use `_camelCase` for private fields (when necessary)
- Use `UPPER_SNAKE_CASE` for constants
- Prefix interfaces with `I` (e.g., `IEnumerable`, `IReusable`, `ISeries`)
- File headers: Document classes per the established XML comments pattern

## Project organization

### Folder structure

The `src/` folder is organized by indicator categories:

- `_common/` - Shared utilities, base classes, BufferList support, catalog, enums, interfaces (`ISeries`, `IReusable`), and common types
- `a-d/` - Indicators A-D (alphabetical)
- `e-k/` - Indicators E-K
- `m-r/` - Indicators M-R
- `s-z/` - Indicators S-Z

All files use the **single namespace** `Skender.Stock.Indicators` declared as `namespace Skender.Stock.Indicators;` (file-scoped namespace, modern C# syntax).

### File organization

- **One public partial class per indicator** spread across multiple files:
  - `{Indicator}.StaticSeries.cs` - Series-style implementation
  - `{Indicator}.StreamHub.cs` - Streaming/real-time implementation
  - `{Indicator}.BufferList.cs` - Buffer-style incremental implementation
  - `{Indicator}.Models.cs` - Result type definitions
  - `{Indicator}.Catalog.cs` - Catalog entry (metadata)
  - `{Indicator}.Utilities.cs` or `{Indicator}.Validation.cs` - Helper methods
- Use consistent file names and `public static partial class {Indicator}` pattern
- Keep each file focused on a single concern

## Performance and optimization

### Numeric precision

- **Use `double` by default** for performance and allocation efficiency
- Use `decimal` ONLY when price-sensitive precision is required (see [Project Principles §1: Mathematical Precision](../../docs/PRINCIPLES.md#1-mathematical-precision-nonnegotiable))
- Never use `float` for indicator values
- Guard division by variable denominators with ternary checks (e.g., `denom != 0 ? num / denom : double.NaN`)
- Document precision requirements and NaN handling in XML comments

### Bounded indicator precision

For indicators with mathematically guaranteed bounds (e.g., 0-100 range for RSI, Stochastic):

#### Guardrails

- **No clamping** - Never use `Math.Clamp()`; boundary violations indicate formula errors
- **No epsilon tolerance** - Bound checks must use exact comparison (`<=`, `>=`, `==`)
- **No forced rounding** - Cannot use rounding to hide precision issues
- **Algorithm-level fixes** - Address root cause in formulas, not symptoms; use algebraically stable algorithms

#### Recommended approach: Boundary detection

For oscillator formulas where values can exactly equal boundaries:

```csharp
// Algebraically stable: detect boundary conditions first
double oscillator;
if (q.Close == highHigh)
{
    oscillator = 100d;  // Exact value, no calculation
}
else if (q.Close == lowLow)
{
    oscillator = 0d;    // Exact value, no calculation
}
else
{
    oscillator = 100d * (q.Close - lowLow) / (highHigh - lowLow);
}
```

**Why this works**:

- Eliminates floating-point division when result is exactly at boundary
- Produces mathematically correct values without precision errors
- Non-boundary calculations remain unchanged

#### Alternative: Formula reformulation

For ratio-based formulas like RSI `100 - 100/(1+rs)`:

```csharp
// Reformulated: Algebraically equivalent, inherently bounded
double rsi = avgLoss > 0
    ? 100d * avgGain / (avgGain + avgLoss)
    : 100;
```

**Why this works**:

- Single division instead of nested operations
- Numerator always ≤ denominator by construction
- Result cannot exceed bounds mathematically

### Collections and LINQ

- **Avoid LINQ in hot loops** (allocation risk); prefer `for` loops
- Use `Span<T>` and `ReadOnlySpan<T>` where applicable for zero-copy operations
- Prefer array allocation and direct indexing over enumerables in performance-critical paths
- Cache collection `.Count` when iterating multiple times
- Use `IReadOnlyList<T>` over `IEnumerable<T>` for API contracts to enable indexing

### Memory efficiency

- Minimize allocations in hot paths
- Use object pooling for frequently created types
- Consider `using` statements for disposable resources
- Document memory behavior in performance-sensitive methods

For performance best practices, use #tool:mslearn to research optimization techniques.

## Input validation and error handling

### Validation

- Validate all public method parameters at the start of the method
- Check for null inputs using `ArgumentNullException.ThrowIfNull(parameter)`
- Check for invalid ranges using `ArgumentOutOfRangeException` with descriptive messages
- Provide parameter-specific validation messages (include the parameter name and constraint)
- Use consistent validation patterns across similar indicators
- Fail fast before allocating large result buffers

### Exception handling

- Use specific exception types (`ArgumentNullException`, `ArgumentOutOfRangeException`, `ArgumentException`)
- Include parameter name and constraint in exception messages
- Document expected exceptions in XML `<exception>` comments
- Do not suppress exceptions silently
- Let exceptions propagate naturally in validation; catch only when necessary for error context

## Documentation

### XML comments

- Document all public types, methods, and properties with `///` comments
- Include `<summary>` tags for all public APIs
- Use `<param>` tags for parameters and `<returns>` for return values
- Document exceptions with `<exception cref="...">` tags
- Use `<inheritdoc/>` for implementations inheriting unchanged semantics
- Keep comments concise and technical
- Use `<remarks>` for additional context on behavior, warmup periods, or streaming specifics

Example:

```csharp
public static partial class Ema
{
    /// <summary>
    /// Converts a list of source data to EMA results.
    /// </summary>
    /// <param name="source">The list of source data.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of EMA results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static IReadOnlyList<EmaResult> ToEma(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods) { }
}
```

## Testing requirements

### Unit tests

- **Inherit from appropriate base classes**: `StaticSeriesTestBase`, `StreamHubTestBase`, or `BufferListTestBase`
- Write tests for all public methods with coverage of:
  - Happy path calculations (Series-based reference data)
  - Edge cases: empty input, minimum/maximum periods, boundary conditions
  - Bad data: null inputs, invalid parameters
  - Insufficient data: inputs smaller than lookback period
  - Mathematical accuracy against manually calculated reference values
- Use MSTest `[TestClass]` and `[TestMethod]` attributes
- Follow test naming convention: `MethodName_StateUnderTest_ExpectedBehavior` (e.g., `ToEma_WithSmallDataset_CalculatesCorrectly`)
- Use FluentAssertions (v6) for readable assertions
- Use precision constants from `TestBase` (e.g., `Money6` and others) for tolerance in epsilon comparisons against manually calculated values only - use the maximum precision needed, not to accommodate algorithmic differences

### Regression tests

- Maintain baseline JSON files in `tests/indicators/_testdata/results/` for validation
- Update baseline files when algorithmic changes occur (use the data generator tool)
- Compare streaming and buffer implementations against Series results for parity validation

For testing best practices, consult #tool:mslearn documentation.

## Building and publishing

### Build configuration

- **Treat all warnings as errors** - ensure solution builds with zero warnings
- Use `dotnet build` for incremental builds during development
- Use `dotnet format --verify-no-changes` to verify code style compliance
- Use `dotnet test` to run unit test suites (use `--settings tests/tests.unit.runsettings`)
- Use `dotnet run --project tools/performance` for performance benchmarking in Release mode
- Support target frameworks: `net10.0`, `net9.0`, `net8.0` (both must build)

### Package metadata

- Keep `.csproj` metadata accurate and up-to-date
- Maintain version numbers per semantic versioning
- Update `docs/_indicators/{Indicator}.md` when indicator APIs change
- Document breaking changes in `src/MigrationGuide.V3.md` and update deprecation bridges in `src/Obsolete.V3.*.cs`

## Common pitfalls to avoid

- **Off-by-one errors in warmup/lookback**: Double-check period calculations against manually verified spreadsheets
- **Null or empty quotes**: Always validate input sequences and handle insufficient data gracefully
- **Precision loss in chained calculations**: Favor `double` for performance; use `decimal` only when necessary
- **Index out of range in streaming**: Guard shared spans and validate cache indices before access
- **Performance regressions from allocations**: Avoid LINQ in hot loops; profile before/after optimization
- **NaN handling**: Use `double.NaN` internally; guard division by zero; convert to `null` only at result boundaries
- **Documentation drift**: Keep `docs/_indicators/*.md` synchronized with code changes
- **Stateful streaming issues**: Ensure buffer state is validated and reset properly; test Insert/Remove scenarios

## Key references and standards

### Governance and architecture

- **[Project Principles](../../docs/PRINCIPLES.md)** - Project principles: Mathematical Precision, Performance First, Comprehensive Validation, Test-Driven Quality, Documentation Excellence, and Scope & Stewardship
- **[NaN handling policy](../../src/_common/README.md#nan-handling-policy)** - Division-by-zero guards, internal NaN propagation, and result boundary conversion
- **[Copilot instructions](../copilot-instructions.md)** - Entry-point guidance for all development areas and custom agents

### Implementation style guides

- **[Series indicators](indicator-series.instructions.md)** - Batch processing baseline implementation (canonical reference for mathematical correctness)
- **[Stream indicators](indicator-stream.instructions.md)** - Real-time stateful StreamHub processing with O(1) optimization patterns
- **[Buffer indicators](indicator-buffer.instructions.md)** - Incremental buffering with BufferList interface patterns
- **[Catalog entries](catalog.instructions.md)** - Indicator metadata and automation configuration

### Formula and validation

- **[Agent instructions](../../src/agents.md)** - CRITICAL formula sourcing hierarchy and mathematical precision requirements for coding agents

### Testing and quality

- **[Source code completion](code-completion.instructions.md)** - Unit testing, code formatting, linting, and pre-commit checklist
- **[Performance testing](performance-testing.instructions.md)** - BenchmarkDotNet guidelines and regression detection

### Tools and research

- #tool:mslearn for C# conventions, .NET best practices, and performance optimization
- #tool:context7 for NuGet package dependencies and external library documentation
- #tool:github/web_search for indicator algorithms and external technical analysis standards

---
Last updated: December 7, 2025
