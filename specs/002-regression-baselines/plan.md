# Implementation plan: static series regression baselines

**Branch**: `plan-regression-tests` | **Date**: 2025-10-06 | **Spec**: [spec.md](./spec.md)

## Summary

Implement a deterministic regression baseline framework for all StaticSeries indicator `Standard` tests. The framework generates JSON files containing canonical indicator outputs, validates current test outputs against these baselines using configurable numeric tolerances, and integrates with CI to detect unintended behavioral drift. The solution consists of a baseline generator tool, a regression test suite, and CI workflow integration.

## Technical context

- **Language/version**: C# / .NET 8.0 and .NET 9.0
- **Primary dependencies**: MSTest, System.Text.Json, existing indicator test infrastructure
- **Storage**: JSON baseline files in `tests/indicators/.baselines/` (version-controlled)
- **Testing**: MSTest regression suite in `tests/indicators/`
- **Target platform**: Multi-target `net8.0;net9.0`
- **Performance goals**: <5 minutes to validate all baselines, <2 minutes to regenerate all baselines
- **Constraints**: Deterministic floating-point behavior, version-controlled baselines, configurable tolerances
- **Scale/scope**: 200+ StaticSeries indicators with Standard test scenarios

## Constitution check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Precision**: Baseline comparisons use double precision with configurable tolerance (default 1e-12). Strict mode available for zero-tolerance validation after intentional algorithm changes. JSON serialization preserves full precision.
- **Performance**: Baseline generation < 2 minutes for all indicators. Regression validation < 5 minutes. Parallel test execution minimizes CI impact. Generator tool optimized for batch operations.
- **Validation**: Comprehensive validation of baseline format, comparison logic, and tolerance handling. Edge tests for missing baselines, null results during warmup, and exact match scenarios. CI integration validates every commit.
- **Test-Driven**: Regression test suite validates all baseline comparisons. Unit tests for BaselineComparer logic (tolerance, strict mode, null handling). Integration tests for generator tool and file format. Performance tests for batch baseline generation.
- **Documentation**: Complete baseline file format specification. Contributor documentation for baseline regeneration procedure. PR review guidance for interpreting regression drift. CI workflow documentation.
- **Scope & Stewardship**: Pure validation layer (no indicator logic changes). No external dependencies beyond existing test infrastructure. Simplicity: JSON format, deterministic serialization, clear tolerance rules.

**Status**: PASS (no violations)

## Project structure

### Documentation (this feature)

```text
specs/002-regression-baselines/
├── spec.md              # Feature specification (input)
├── plan.md              # This file (implementation plan)
└── tasks.md             # Actionable task breakdown

```

### Source code (repository root)

```text
tools/performance/BaselineGenerator/
├── BaselineGenerator.csproj          # Console tool project
├── Program.cs                        # Entry point, CLI argument handling
└── IndicatorExecutor.cs              # Test execution and output capture

tests/indicators/
├── s-z/Sma/
│   ├── Sma.Baseline.json             # SMA baseline (colocated with tests)
│   └── Sma.StaticSeries.Tests.cs     # SMA tests
├── m-r/Macd/
│   ├── Macd.Baseline.json            # MACD baseline (colocated with tests)
│   └── Macd.StaticSeries.Tests.cs    # MACD tests
├── RegressionTests.cs                # Regression test suite
└── BaselineComparer.cs               # Comparison logic with tolerance

.github/workflows/
└── test-indicators.yml               # Updated to include regression tests
```

**Structure decision**: Baseline files colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`). Direct JSON serialization of result arrays (e.g., `List<SmaResult>`). Generator tool as standalone console app in `tools/performance/`. Regression tests integrated into existing test project.

## Phase 0: Research and design decisions

No external research required—design extends existing test infrastructure patterns.

### Key decisions

- **JSON file format rationale**:
  - Human-readable for code review during baseline regeneration
  - System.Text.Json for .NET standard serialization
  - Deterministic property order for minimal diff noise
  - camelCase convention matches JavaScript/TypeScript ecosystem
  - One file per indicator enables parallel baseline generation and clear git history

- **Numeric tolerance strategy**:
  - Default tolerance 1e-12 accounts for acceptable floating-point precision differences
  - Strict mode (tolerance = 0) for validating intentional algorithm changes
  - Configurable per-test to handle indicators with different precision requirements
  - Comparison uses absolute difference (not relative percentage) for simplicity

- **Baseline file location**:
  - `tests/indicators/.baselines/` co-locates baselines with test code
  - `.gitignore` exception ensures baseline files are version-controlled
  - One JSON file per indicator (e.g., `Sma.Standard.json`) enables selective regeneration
  - Directory structure flat (no subdirectories) for simplicity

- **Generator tool architecture**:
  - Standalone console app in `tools/performance/BaselineGenerator/`
  - Leverages existing test data (TestData.GetDefault()) to avoid duplication
  - Batch mode generates all baselines in parallel for performance
  - Single-indicator mode for selective regeneration during development

- **CI integration approach**:
  - Regression tests run in existing `test-indicators.yml` workflow
  - Environment variable `RUN_REGRESSION_TESTS=true` gates execution (opt-in for now)
  - Baseline drift fails build (prevents merging unintended changes)
  - CI logs drift summary (count of mismatches, affected indicators) for visibility

**Output**: No separate research.md needed (decisions captured above)

## Phase 1: Design and contracts

Prerequisites: research.md complete (decisions documented in Phase 0 above)

### Data model

**Entities**:

- **Baseline files**: Direct JSON serialization of indicator result arrays
  - Format: `List<TResult>` where `TResult` is the indicator's result type (e.g., `SmaResult`, `MacdResult`)
  - Location: Colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
  - No custom wrapper models needed - uses existing result types directly
- **BaselineComparer**: Comparison logic
  - `Compare(expected, actual, tolerance)` → ComparisonResult
  - `ComparisonResult`: { IsMatch, Mismatches: List of MismatchDetail }
  - `MismatchDetail`: { Date, PropertyName, Expected, Actual, Delta }
- **BaselineGenerator tool**: Console application (located in `tools/performance/BaselineGenerator/`)
  - `GenerateBaseline(indicatorName)` → `List<TResult>`
  - Direct JSON serialization using `System.Text.Json`
  - CLI: `--indicator <name>` | `--all`

### API contracts

**Baseline JSON format**:

```json
[
  {
    "timestamp": "2016-01-04T00:00:00",
    "sma": null
  },
  {
    "timestamp": "2016-01-05T00:00:00",
    "sma": null
  },
  {
    "timestamp": "2016-02-01T00:00:00",
    "sma": 214.52
  },
  {
    "timestamp": "2016-02-02T00:00:00",
    "sma": 215.18
  }
]
```

**Direct deserialization**:

```csharp
// Load and deserialize baseline using standard JSON serialization
var options = new JsonSerializerOptions 
{ 
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
};
List<SmaResult> baseline = JsonSerializer.Deserialize<List<SmaResult>>(json, options);
```

**BaselineComparer API**:

```csharp
public class BaselineComparer
{
    public ComparisonResult Compare(
        IEnumerable<BaselineResult> expected,
        IEnumerable<BaselineResult> actual,
        double tolerance = 1e-12);
        
    public ComparisonResult CompareStrict(
        IEnumerable<BaselineResult> expected,
        IEnumerable<BaselineResult> actual);
}

public record ComparisonResult(
    bool IsMatch,
    List<MismatchDetail> Mismatches);

public record MismatchDetail(
    DateTime Date,
    string PropertyName,
    double? Expected,
    double? Actual,
    double? Delta);
```

**BaselineGenerator tool CLI**:

```bash
# Generate single indicator baseline (colocated with tests)
dotnet run --project tools/performance/BaselineGenerator -- --indicator Sma

# Generate all baselines (colocated with tests)
dotnet run --project tools/performance/BaselineGenerator -- --all
```

**Output location**: Files are automatically colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)

### Test scenarios

**Unit Tests (BaselineComparer)**:

- Compare identical results → IsMatch = true, Mismatches empty
- Compare results within tolerance → IsMatch = true
- Compare results exceeding tolerance → IsMatch = false, Mismatches populated
- Strict mode with exact match → IsMatch = true
- Strict mode with any difference → IsMatch = false
- Handle null values during warmup period → Compare nulls as equal
- Handle missing properties → Report as mismatch
- Handle date misalignment → Report missing/extra dates

**Integration Tests (BaselineGenerator)**:

- Generate baseline for single indicator → Valid JSON file created
- Generate baselines for all indicators → All JSON files created (200+)
- Regenerate existing baseline → File overwritten with new timestamp
- Handle missing test data → Graceful error message
- Validate JSON schema → All required metadata fields present
- Deterministic output → Multiple runs produce identical JSON (except timestamp)

**Regression Tests (RegressionTestSuite)**:

- Load baseline, run indicator, compare outputs → Test passes
- Baseline drift introduced → Test fails with clear diagnostics
- Missing baseline file → Test skipped with warning (not failure)
- Strict mode enabled → Zero tolerance enforced
- Warmup nulls handled → Nulls compared correctly

**Performance Tests**:

- Generate all baselines in < 2 minutes
- Run all regression tests in < 5 minutes
- Parallel baseline generation scales linearly with CPU cores

### Implementation guidelines

**Baseline file format**:

- Use System.Text.Json with JsonSerializerOptions configured for:
  - PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  - WriteIndented = true (for human readability)
  - DefaultIgnoreCondition = JsonIgnoreCondition.Never (serialize nulls)
  - Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping (minimal escaping)
- Deterministic property order: alphabetical by property name
- Date format: ISO 8601 (yyyy-MM-dd) for consistency with existing test data
- Metadata stored in `metadata` object, results in `results` array

**Numeric comparison logic**:

- Absolute difference: `Math.Abs(expected - actual) > tolerance` determines mismatch
- Null handling: null == null → match, null != value → mismatch
- Tolerance applied per-property (not per-result object)
- Strict mode: tolerance = 0, enforces exact binary equality

**Generator tool design**:

- Leverage existing test data: `TestData.GetDefault()` for Standard scenarios
- Execute indicator method: Invoke `quotes.ToIndicator()` to generate outputs
- Capture all result properties via reflection: Iterate result object properties (excluding Date)
- Serialize to JSON: Use BaselineWriter with deterministic options
- Parallel generation: Use `Parallel.ForEach` for `--all` mode
- CLI argument parsing: Use System.CommandLine or simple args array

**Regression test integration**:

- One test method per indicator: `[TestMethod] public void Sma_Standard_RegressionTest()`
- Test discovery via reflection: Enumerate all indicators from catalog or namespace
- Baseline loading: Deserialize JSON file using BaselineReader
- Test execution: Run indicator with Standard test data
- Comparison: Use BaselineComparer with default tolerance
- Failure diagnostics: Assert with custom message listing all mismatches
- Missing baseline handling: `Assert.Inconclusive("Baseline file not found: Sma.Standard.json")`

### Documentation updates

- Add baseline regeneration procedure to `contributing.md` (when to regenerate, how to run generator)
- Add regression testing section to `README.md` (what baselines are, how they protect against drift)
- Create `docs/baseline-format.md` with JSON schema specification and examples
- Update PR review checklist in `contributing.md` to include baseline drift review
- Document CI environment variables in `.github/workflows/README.md` (RUN_REGRESSION_TESTS flag)

### Quality gates and conformance

- Run baseline generation for all indicators to establish initial "known good" outputs
- Commit baseline JSON files to version control (one commit per batch for clarity)
- Execute regression test suite to verify all baselines validate successfully
- Document baseline drift review process for PR reviewers (how to interpret failures, when to regenerate)

## Phase 2: Task planning approach

*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task generation strategy**:

- Load Phase 1 contracts and data model
- Generate tasks grouped by component:
  1. **Infrastructure tasks**: Baseline file format definition, JSON serialization utilities
  2. **Generator tool tasks**: CLI scaffolding, indicator execution, batch processing
  3. **Comparer tasks**: Tolerance logic, mismatch detection, strict mode
  4. **Test suite tasks**: Regression test implementation, baseline loading, failure diagnostics
  5. **CI integration tasks**: Workflow updates, environment variable gating, result publishing
  6. **Documentation tasks**: Contributor guides, format specification, PR review guidance
- Mark tasks parallelizable ([P]) when independent (e.g., generator tool and comparer logic)
- Sequential within each component (tests depend on implementation)

**Ordering strategy**:

- Infrastructure first (baseline format, JSON utilities)
- Then generator tool (to create initial baselines) and comparer (comparison logic) in parallel
- Regression test suite after comparer is complete
- CI integration after regression tests validate locally
- Documentation throughout (can run parallel to implementation)

**Estimated output**: ~30 tasks (infrastructure + generator + comparer + tests + CI + docs)

## Phase 3+: Future implementation

*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, performance validation, documentation review)

## Complexity tracking

*Fill ONLY if Constitution Check has violations that must be justified*

No violations—section intentionally empty.

## Progress tracking

*This checklist is updated during execution flow*

**Phase status**:

- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [ ] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate status**:

- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved
- [x] Complexity deviations documented

---

*Based on Constitution v1.1.0 - See `/memory/constitution.md`*

---
Last updated: October 6, 2025
