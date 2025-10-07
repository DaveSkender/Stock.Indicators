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
├── BaselineWriter.cs                 # JSON serialization and file writing
└── IndicatorExecutor.cs              # Test execution and output capture

tests/indicators/
├── .baselines/                       # Generated baseline JSON files
│   ├── Adl.Standard.json
│   ├── Adx.Standard.json
│   ├── Sma.Standard.json
│   └── ... (one per indicator)
├── RegressionTests.cs                # Regression test suite
├── BaselineComparer.cs               # Comparison logic with tolerance
└── BaselineReader.cs                 # JSON deserialization

.github/workflows/
└── test-indicators.yml               # Updated to include regression tests
```

**Structure decision**: Single baseline file per indicator in dedicated `.baselines/` directory. Generator tool as standalone console app in `tools/performance/`. Regression tests integrated into existing test project.

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

- **BaselineFile**: Root object containing metadata + results array
  - `Metadata`: { indicatorName, scenarioName, generatedAt, libraryVersion, warmupPeriodCount }
  - `Results`: Array of BaselineResult objects (one per date)
- **BaselineResult**: Single result entry
  - `Date`: DateTime in ISO 8601 format
  - `Properties`: Dictionary<string, double?> for all result properties (e.g., `{ "sma": 214.52 }`)
- **BaselineComparer**: Comparison logic
  - `Compare(expected, actual, tolerance)` → ComparisonResult
  - `ComparisonResult`: { IsMatch, Mismatches: List of MismatchDetail }
  - `MismatchDetail`: { Date, PropertyName, Expected, Actual, Delta }
- **BaselineGenerator tool**: Console application (located in `tools/performance/BaselineGenerator/`)
  - `GenerateBaseline(indicatorName)` → BaselineFile
  - `WriteBaseline(BaselineFile, filePath)`
  - CLI: `--indicator <name>` | `--all` | `--output <dir>`

### API contracts

**Baseline JSON format**:

```json
{
  "metadata": {
    "indicatorName": "SMA",
    "scenarioName": "Standard",
    "generatedAt": "2025-10-06T12:34:56Z",
    "libraryVersion": "3.0.0",
    "warmupPeriodCount": 19
  },
  "results": [
    { "date": "2016-01-04", "sma": null },
    { "date": "2016-01-05", "sma": null },
    ...
    { "date": "2016-02-01", "sma": 214.52 },
    { "date": "2016-02-02", "sma": 215.18 }
  ]
}
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
# Generate single indicator baseline
dotnet run --project tools/performance/BaselineGenerator -- --indicator SMA

# Generate all baselines
dotnet run --project tools/performance/BaselineGenerator -- --all

# Specify output directory
dotnet run --project tools/performance/BaselineGenerator -- --all --output tests/indicators/.baselines/
```

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
