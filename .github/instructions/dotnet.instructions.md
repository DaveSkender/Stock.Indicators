---
applyTo: "src/**"
description: ".NET development and coding standards"
---

# .NET development instructions

These instructions apply to all files in the `src/` folder and cover C# coding standards, project organization, and .NET best practices for the Stock Indicators library.

## Code style and formatting

### C# language features

- Use modern C# features (target language version matches the framework)
- Use `nullable` reference types throughout
- Apply `required` keyword for mandatory initialization
- Use pattern matching and switch expressions where appropriate
- Use records for immutable data structures when suitable

### Code formatting

- Follow the `.editorconfig` configuration for consistent formatting
- Run `dotnet format` before committing code
- Use `var` for obvious types, explicit types for clarity
- Prefer expression bodies for simple methods and properties
- Use `#pragma` directives sparingly for suppressing warnings

For detailed style guidance, consult the official C# coding conventions via #tool:mslearn.

### Naming conventions

- Use `PascalCase` for public types, methods, and properties
- Use `camelCase` for local variables and parameters
- Use `_camelCase` for private fields
- Use `UPPER_SNAKE_CASE` for constants
- Prefix interfaces with `I` (e.g., `IEnumerable`)

## Project organization

### Folder structure

The `src/` folder is organized by indicator categories:

- `_common/` - Shared utilities, base classes, and common types
- `a-d/` - Indicators A-D (alphabetical)
- `e-k/` - Indicators E-K
- `m-r/` - Indicators M-R
- `s-z/` - Indicators S-Z

### File organization

- One public class per file (exceptions: tightly coupled types)
- Group related types in namespaces
- Use consistent file names matching the primary class

## Performance and optimization

### Data type selection

- **Public API**: Use `decimal` for quote inputs (`IQuote` interface: Open, High, Low, Close, Volume)
- **Internal calculations**: Use `double` for performance via `QuoteD` and `.ToQuoteD()` converter methods
- **Result types**: Most indicators use `double?` implementing `IReusableResult.Value`, but use `decimal?` when precision is critical:
  - `decimal?` examples: ZigZag, PivotPoints, HeikinAshi, Renko (preserves exact price levels)
  - `double?` examples: Moving averages, oscillators, volume indicators (calculated values)
- **Never use `float`** for indicator values
- Document precision requirements in XML comments

### Collections and LINQ

- Avoid excessive LINQ chaining (readability over chaining)
- Prefer direct loops for performance-critical code
- Use `Span<T>` and `ReadOnlySpan<T>` for zero-copy operations
- Cache collection `.Count` when iterating multiple times

### Memory efficiency

- Minimize allocations in hot paths
- Use object pooling for frequently created types
- Consider `using` statements for disposable resources
- Document memory behavior in performance-sensitive methods

For performance best practices, use #tool:mslearn to research optimization techniques.

## Input validation and error handling

### Validation

- Validate all public method parameters
- Check for null inputs, negative values, and invalid ranges
- Throw `ArgumentException` for invalid inputs with descriptive messages
- Provide clear error messages to help users understand the issue

### Exception handling

- Use specific exception types (avoid bare `Exception`)
- Include context in exception messages
- Document expected exceptions in XML comments
- Do not suppress exceptions silently

## Documentation

### XML comments

- Document all public types and methods
- Include `<summary>`, `<param>`, `<returns>`, and `<remarks>` tags
- Document exceptions with `<exception>` tags
- Provide usage examples in `<example>` tags when helpful
- Keep comments concise and technical

Example:

```csharp
/// <summary>
/// Calculates the moving average indicator.
/// </summary>
/// <param name="quotes">Historical quote data with required fields.</param>
/// <param name="period">Number of periods for the moving average.</param>
/// <returns>A collection of moving average results.</returns>
/// <exception cref="ArgumentException">
/// Thrown when period is less than 1 or greater than quote count.
/// </exception>
public static IEnumerable<MAResult> GetMovingAverage(
    IEnumerable<Quote> quotes,
    int period) { }
```

## Testing requirements

### Unit tests

- Write tests for all public methods
- Cover edge cases: empty input, minimum/maximum values, boundary conditions
- Use descriptive test names that explain the scenario
- Keep tests focused on single behaviors
- Mock external dependencies appropriately

### Regression tests

- Maintain baseline data for indicator validation
- Update baseline files when algorithmic changes occur
- Use the test data generator tool for creating baseline JSON files

For testing best practices, consult #tool:mslearn documentation.

## Building and publishing

### Build configuration

- Ensure solution builds without warnings
- Use `dotnet build` for local development
- Use `dotnet test` to run all test suites
- Use `dotnet pack` to create NuGet packages

### Package metadata

- Keep `.csproj` metadata accurate and up-to-date
- Maintain version numbers per semantic versioning
- Document breaking changes in release notes

## Referencing external standards

For comprehensive C# and .NET best practices, use #tool:mslearn to research:

- Official C# coding conventions
- Performance optimization techniques
- Security best practices
- Asynchronous programming patterns
- Design patterns and principles

---
Last updated: December 7, 2025
