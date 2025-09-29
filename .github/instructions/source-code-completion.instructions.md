---
applyTo: "src/**,tests/**"
description: ".NET development, testing, formatting, linting, and performance guidelines for Stock Indicators library"
---

# Source code, testing, and pre-commit code completion checklist

These instructions apply to all files in the `src/` and `tests/` folders and cover .NET development, unit testing, performance benchmarking, code formatting, and library-specific patterns. Use the VS Code tasks **Restore: .NET Packages**, **Build: .NET Solution**, **Test: .NET Solution**, and **Lint: Markdown Files** (`Ctrl+Shift+P` → `Tasks: Run Task`) whenever you prefer a guided experience over CLI commands.

## Coding agent assistant reminders

- **Critical for coding agents**: Complete this entire checklist before marking work as finished, requesting review, or declaring a pull request ready.
- Fix every error or warning that appears during the process. Do not suppress analyzers, compiler warnings, or markdown lint issues unless a maintainer explicitly directs you to.
- Restart from Preparation Step 1 whenever any verification fails.
- Prefer the provided VS Code tasks for repeatable execution of restore, build, test, and markdown lint commands.

## Preparation steps

### Step 1 – Cleanup: Remove all dead code

- Delete obsolete or commented-out code, unused imports, variables, helper methods, and temporary scripts.
- Remove scratch files (`.bak`, `.new`, `.debug.*`), empty directories, and TODO-style notes resolved during development.
- Strip debugging aids such as `Console.WriteLine`, ad-hoc harness code, and leftover breakpoints.

> **Verification**: Run `dotnet format --verify-no-changes` to expose unexpected edits and review `git diff` for lingering debris.

## Validation steps

### Step 1 – Run prerequisites

- Run `dotnet restore` (or the **Restore: .NET Packages** task).
- Run `dotnet format` to auto-correct stylistic issues.
- Run `npm run lint:md:fix` whenever documentation changed.
- If tooling changed, rerun `npm install` so markdown tooling stays in sync.

### Step 2 – Formatting and analyzer checks

- Execute `dotnet format --verify-no-changes` to confirm .editorconfig compliance.
- Run `dotnet build "Stock.Indicators.sln" -v minimal --nologo` and resolve every warning.
- Do not introduce suppressions unless documented and approved.

> **Verification**: Repeat `dotnet format --verify-no-changes` and `dotnet build` until both finish with zero warnings or errors.

### Step 3 – Building

- Build in both Debug and Release when performance-sensitive code changed: `dotnet build "Stock.Indicators.sln" -c Release -v minimal --nologo`.
- Ensure auxiliary projects (benchmarks, examples, docs tooling) still compile.

> **Verification**: Confirm `dotnet build "Stock.Indicators.sln" -c Release -v minimal --nologo` finishes without warnings.

### Step 4 – Testing

- Run unit tests: `dotnet test "Stock.Indicators.sln" --no-restore --nologo`.
- Collect coverage: `dotnet test "Stock.Indicators.sln" --no-restore --collect:"Code Coverage" --nologo`.
- Add or update tests until coverage remains above the 98% project expectation.
- Run performance tests when applicable: `cd tests/performance && dotnet run -c Release`.

> **Verification**: Repeat the affected test runs until all pass and coverage reports show no regressions.

## Completion steps

### Step 1 – Documentation: Update everything that changed

- Refresh XML documentation, inline comments, and user-facing content under `docs/`, especially indicator reference pages.
- Always update `/docs` for individual indicators when adding or updating.
- Keep wording consistent across source code and documentation.

> **Verification**: Manually review updated documentation against the implemented behavior.

### Step 2 – Lint markdown documentation

- Run `npm run lint:md:fix` to auto-correct basic issues.
- Run `npm run lint:md` (or the **Lint: Markdown Files** task) and fix remaining warnings.

> **Verification**: `npm run lint:md` exits with zero warnings.

### Step 3 – Final verification: Perform a full clean sweep

Run this sequence and confirm each command completes successfully.

```bash
dotnet format --verify-no-changes
dotnet build "Stock.Indicators.sln" -v minimal --nologo
dotnet test "Stock.Indicators.sln" --no-build --nologo
npm run lint:md
```

If any command fails, fix the issue and restart this step from the top.

### Step 4 – Optional but recommended: Codacy checks

Run optional Codacy checks for additional code quality insights:

```bash
# Install Codacy CLI if not already installed (optional)
curl -L https://github.com/codacy/codacy-analysis-cli/releases/latest/download/codacy-analysis-cli-linux -o codacy-analysis-cli
chmod +x codacy-analysis-cli

# Run Codacy analysis (optional)
./codacy-analysis-cli analyze --project-token [YOUR_TOKEN] --directory .
```

### Step 5 – Manual inspection of documentation site

- Start the site via the **Run: Doc Site with LiveReload** task (runs `bundle exec jekyll serve`).
- Inspect key pages using the Playwright MCP server or a browser to confirm layout and content.
- Stop the server with `Ctrl+C` after verification.

### Step 6 – Golden test: Prove a clean rebuild succeeds

Run the following sequence to detect hidden state issues.

```bash
npm run clean
dotnet restore
dotnet build "Stock.Indicators.sln" -v minimal --nologo
dotnet test "Stock.Indicators.sln" --no-build --nologo
```

- If benchmarks or docs packaging were affected, rerun `dotnet run -c Release` from `tests/performance` and `bundle exec jekyll build`.

### Step 7 – Sign-off: Confirm completion

- [ ] All dead or debugging code removed
- [ ] `dotnet format --verify-no-changes` passes without edits
- [ ] `dotnet build` succeeds with zero warnings
- [ ] All required test suites pass
- [ ] Coverage meets or exceeds the 98% goal
- [ ] Documentation and XML comments updated
- [ ] Markdown linting succeeds
- [ ] Documentation site inspected (when changed)
- [ ] Golden test sequence passes

> **Important**: If you had to make additional changes during sign-off, return to Step 2 of the validation checklist and repeat the flow.

## Advanced safeguards

- Configure optional git pre-commit hooks to run `dotnet format`, `dotnet build`, and `dotnet test`.
- Monitor the VS Code Problems panel and ensure it is empty before requesting review.
- For complex features, prefer staged pull requests so reviewers can validate incremental progress.

## Reference workflows

### Build and development workflow

#### Core development commands

```bash
# build the entire solution
dotnet build

# build in release mode
dotnet build -c Release

# restore NuGet packages
dotnet restore
```

#### Code cleanup and formatting

```bash
# format all code using .editorconfig rules
dotnet format

# format with verification (non-destructive check)
dotnet format --verify-no-changes

# format specific projects
dotnet format src/Indicators.csproj
dotnet format tests/indicators/Tests.Indicators.csproj
```

### Testing guidelines

#### Unit testing workflow

```bash
# run all unit tests
dotnet test

# run unit tests only (excludes integration tests)
dotnet test --settings tests/tests.unit.runsettings

# run integration tests only
dotnet test --settings tests/tests.integration.runsettings

# run with coverage reporting
dotnet test --collect:"Code Coverage"

# generate detailed coverage report
dotnet test --collect:"XPlat Code Coverage" --results-directory:./coverage
reportgenerator -reports:./coverage/*/*.xml -targetdir:./coverage/report -reporttypes:Html
```

> **Code Coverage**: Maintain 98% line coverage. Use the generated HTML report to identify uncovered lines and add appropriate tests.

#### Test organization requirements

- **Unit tests**: Place in `tests/indicators/` following existing structure
- **Performance tests**: Use `tests/performance/` for benchmark tests
- **Integration tests**: Mark with `[TestCategory("Integration")]` attribute
- **External tests**: Place API tests in `tests/external/` folders

#### Test patterns and standards

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

### Performance testing

#### Benchmark execution

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

#### Performance considerations

- Use `double` precision, and limit `decimal` to price-sensitive monetary calculations
- Minimize LINQ chaining in hot paths
- Implement efficient lookback period calculations
- Profile memory allocation patterns
- Test with realistic data volumes

### Code quality standards

#### Required practices

- **Input validation**: Validate all public method parameters
- **XML documentation**: All public methods must have complete XML docs
- **Exception handling**: Use consistent error messages and exception types
- **Mathematical accuracy**: Test against proven reference implementations
- **Null safety**: Handle insufficient data scenarios gracefully

#### Financial calculation guidelines

- Use `double` precision, and limit `decimal` to price-sensitive monetary calculations
- Handle `double.NaN` appropriately in result types
- Implement `IReusable` interface correctly for chainable indicators
- Follow established patterns for lookback period validation
- Ensure calculation accuracy matches reference implementations

#### Code organization patterns

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

### Library-specific requirements

#### Indicator development checklist

- [ ] Implement series-style indicator method
- [ ] Create result model inheriting from `IReusable` or `ISeries`
- [ ] Add comprehensive unit tests with manual calculations
- [ ] Include performance benchmark if computationally intensive
- [ ] Create catalog entry following established patterns
- [ ] Update documentation in `docs/_indicators/`
- [ ] Validate mathematical accuracy against reference sources

#### Common pitfalls to avoid

1. **Off-by-one errors** in lookback period calculations
2. **Null reference exceptions** with insufficient data
3. **Precision loss** in price-sensitive calculations – default to `double` for performance and switch to `decimal` when additional precision materially impacts results
4. **Index out of bounds** when accessing historical data
5. **Performance regression** from excessive LINQ operations

#### Result model patterns

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

### Linting and static analysis

#### Code analysis tools

- **EditorConfig**: Follow formatting rules in `.editorconfig`
- **Roslyn analyzers**: Address all compiler warnings
- **Code coverage**: Maintain high test coverage (98%)
- **Security scanning**: Use tools like Codacy for vulnerability detection

#### Pre-commit validation

Before committing source code changes:

1. **Build verification**: `dotnet build` succeeds without warnings
2. **Format check**: `dotnet format --verify-no-changes` passes
3. **Unit tests**: All existing tests continue to pass
4. **New tests**: Add tests for any new functionality
5. **Performance check**: Run relevant benchmarks if performance-critical

#### Continuous integration requirements

- All builds must complete without errors or warnings
- Unit test coverage must not decrease
- Performance benchmarks must not regress significantly
- Security scans must not introduce new vulnerabilities

---
Last updated: September 28, 2025
