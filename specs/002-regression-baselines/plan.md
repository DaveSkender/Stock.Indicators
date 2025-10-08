# Implementation plan: static series regression baselines

**Branch**: `plan-regression-tests` | **Date**: 2025-10-06 | **Spec**: [spec.md](./spec.md)

## Summary

Implement a deterministic regression baseline framework for all StaticSeries indicator `Standard` tests. The framework generates JSON files containing canonical indicator outputs, validates current test outputs against these baselines using exact binary equality, and integrates with CI to detect unintended behavioral **drift** (deviations from expected values). Any drift is unacceptable and must be investigated—either corrected if unintentional, or baselines regenerated if the change is deliberate. The solution consists of a baseline generator tool, a baseline comparison utility, a regression test suite, and CI workflow integration.

## Technical context

- **Language/version**: C# / .NET 8.0 and .NET 9.0
- **Primary dependencies**: MSTest, System.Text.Json, existing indicator test infrastructure
- **Storage**: JSON baseline files in `tests/indicators/.baselines/` (version-controlled)
- **Testing**: MSTest regression suite in `tests/indicators/`
- **Target platform**: Multi-target `net8.0;net9.0`
- **Performance goals**: <5 minutes to validate all baselines, <2 minutes to regenerate all baselines
- **Constraints**: Deterministic floating-point behavior, version-controlled baselines, exact binary equality
- **Scale/scope**: 200+ StaticSeries indicators with Standard test scenarios

## Constitution check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Mathematical Precision (NON-NEGOTIABLE)**: Baseline comparisons enforce mathematical correctness by detecting ANY deviation from validated outputs. JSON serialization preserves full double precision (15-17 significant digits). Comparisons use exact binary equality—no tolerance, no approximation. If platform or .NET version differences exist, they indicate bugs in the indicator implementation that must be fixed (Constitution Principle I is NON-NEGOTIABLE). No rounding occurs during serialization or comparison—values stored and compared at maximum precision.

- **Performance First**: Baseline generation completes in <2 minutes for all 200+ indicators. Regression validation runs in <5 minutes to minimize CI impact. Generator tool uses parallel execution (`Parallel.ForEach`) for batch operations. Baseline files are flat JSON (no nested objects) for fast deserialization. Comparison logic uses O(n) single-pass iteration with early exit on first mismatch in non-verbose mode. No LINQ in per-comparison hot paths—direct iteration for optimal performance.

- **Comprehensive Validation**: Baseline comparer validates null handling, missing properties, and date misalignment with comprehensive unit tests. Generator tool validates test data availability, indicator execution success, and JSON schema conformance. Regression tests detect missing baseline files (skipped with warning) vs. actual drift (hard failure). CI integration validates every commit—baseline drift blocks merge. All edge cases documented: warmup nulls, exact zero values, extreme precision scenarios.

- **Test-Driven Quality**: TDD workflow mandatory: unit tests for BaselineComparer logic (null comparison, mismatch detection) written before implementation. Integration tests for BaselineGenerator tool (single indicator, batch mode, file format validation). Regression test suite validates all 200+ indicators against baselines. Performance tests validate generation and comparison speed targets. Each baseline file serves as executable documentation of expected behavior. **Task ordering enforces Red→Green→Refactor**: test tasks precede implementation tasks (T019→T016, test subtasks before T008/T012).

- **Documentation Excellence**: Complete baseline file format specification in `docs/baseline-format.md` with JSON schema and examples. Contributor documentation in `contributing.md` covers baseline regeneration procedure (when, why, how). PR review guidance explains interpreting baseline drift failures. CI workflow documentation describes environment variables and failure diagnostics. XML documentation for all public comparison APIs (BaselineComparer, ComparisonResult, MismatchDetail).

- **Scope & Stewardship**: Pure validation layer—no changes to indicator calculation logic. Zero external dependencies beyond existing test infrastructure (MSTest, System.Text.Json). Baselines version-controlled for transparency and auditability. Simple JSON format enables human code review during regeneration. One baseline file per indicator provides clear git history and selective regeneration capability. Supports all 200+ indicators uniformly without special-casing instrument types.

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

**Updated per PR #1496** (simplified approach):

```text
tools/performance/BaselineGenerator/
├── BaselineGenerator.csproj          # Console tool project
├── Program.cs                        # Entry point, CLI, serialization
└── IndicatorExecutor.cs              # Test execution and output capture

tests/indicators/
├── a-d/
│   ├── Adl/
│   │   ├── Adl.Tests.cs
│   │   └── Adl.Baseline.json         # Colocated baseline
│   └── Adx/
│       ├── Adx.Tests.cs
│       └── Adx.Baseline.json
├── s-z/
│   └── Sma/
│       ├── Sma.Tests.cs
│       └── Sma.Baseline.json
├── RegressionTests.cs                # Regression test suite
└── BaselineComparer.cs               # Comparison utility

.github/workflows/
└── test-indicators.yml               # Updated to include regression tests
```

**Structure decision**: Baseline files colocated with indicator tests (not separate directory). No custom models or serialization utilities—uses standard System.Text.Json to deserialize directly to `List<TResult>`. Generator tool as standalone console app in `tools/performance/`.

## Phase 0: Research and design decisions

No external research required—design extends existing test infrastructure patterns.

### Key decisions

- **JSON file format rationale** (simplified per PR #1496):
  - Direct deserialization to `List<TResult>` (no wrapper models)
  - Human-readable for code review during baseline regeneration
  - System.Text.Json for .NET standard serialization
  - Deterministic output for minimal diff noise
  - camelCase convention matches JSON standards
  - One file per indicator enables parallel baseline generation and clear git history

- **Exact binary equality requirement**:
  - All comparisons enforce exact binary equality (zero tolerance)
  - Mathematical precision is NON-NEGOTIABLE per Constitution Principle I
  - Any platform or .NET version difference indicates a bug requiring investigation
  - Baseline framework detects ALL changes, no matter how small
  - If indicator outputs differ, developer must either fix the bug or deliberately regenerate baselines with justification

- **Baseline file location** (updated per PR #1496):
  - Colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
  - `.gitignore` exception ensures baseline files are version-controlled
  - One JSON file per indicator (e.g., `Sma.Baseline.json`) enables selective regeneration
  - No separate `.baselines/` directory—files live alongside their indicator tests

- **Generator tool architecture**:
  - Standalone console app in `tools/performance/BaselineGenerator/`
  - Leverages existing test data (TestData.GetDefault()) to avoid duplication
  - Uses standard System.Text.Json serialization (no custom utilities per PR #1496)
  - Batch mode generates all baselines in parallel for performance
  - Single-indicator mode for selective regeneration during development

- **CI integration approach**:
  - Regression tests run in existing `test-indicators.yml` workflow
  - Environment variable `RUN_REGRESSION_TESTS=true` gates execution (opt-in for now)
  - **Drift fails build immediately**—any deviation from baselines is unacceptable and blocks merge
  - CI logs drift summary (count of mismatches, affected indicators, severity) for investigation
  - Developer must either fix the code change or deliberately regenerate baselines with justification

**Output**: No separate research.md needed (decisions captured above)

## Phase 1: Design and contracts

Prerequisites: research.md complete (decisions documented in Phase 0 above)

### Data model

**Note**: PR #1496 simplified the approach to use direct result class mapping instead of custom wrapper models.

**Simplified Entities** (per PR #1496):

- **Baseline JSON files**: Direct JSON arrays of indicator result objects
  - Format: `List<TResult>` where TResult is the indicator's result class (e.g., `List<SmaResult>`)
  - Location: Colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
  - No metadata wrapper—just arrays of results with timestamp and properties
- **BaselineComparer**: Comparison utility
  - `Compare(expected, actual)` → ComparisonResult
  - `ComparisonResult`: { IsMatch, Mismatches: List of MismatchDetail }
  - `MismatchDetail`: { Timestamp, PropertyName, Expected, Actual, Delta }
  - **Terminology**: "Mismatch" = any comparison difference; "Drift" = unacceptable deviation requiring action
- **BaselineGenerator tool**: Console application (located in `tools/performance/BaselineGenerator/`)
  - Executes indicators with Standard test data
  - Serializes results directly using System.Text.Json
  - CLI: `--indicator <name>` | `--all` | `--output <dir>`

### API contracts

**Baseline JSON format** (simplified per PR #1496):

```json
[
  {
    "timestamp": "2016-01-04T00:00:00",
    "sma": null
  },
  {
    "timestamp": "2016-02-01T00:00:00",
    "sma": 214.52331349206352
  }
]
```

**BaselineComparer API** (updated for simplified format):

```csharp
public class BaselineComparer
{
    public ComparisonResult Compare<TResult>(
        IEnumerable<TResult> expected,
        IEnumerable<TResult> actual)
        where TResult : IReusableResult;
}

public record ComparisonResult(
    bool IsMatch,
    List<MismatchDetail> Mismatches);

public record MismatchDetail(
    DateTime Timestamp,
    string PropertyName,
    double? Expected,
    double? Actual,
    double? Delta);
```

**Note on Terminology**:

- **"Mismatch"**: Any comparison difference detected by the comparer
- **"Drift"**: Unacceptable deviation from expected baseline values that requires investigation and correction. Any mismatch in CI is drift and blocks merge—either the code change must be fixed or baselines must be deliberately regenerated.

**BaselineGenerator tool CLI**:

```bash
# Generate single indicator baseline  
dotnet run --project tools/performance/BaselineGenerator -- --indicator Sma

# Generate all baselines (colocated with tests)
dotnet run --project tools/performance/BaselineGenerator -- --all

# Specify output directory (defaults to colocated with tests)
dotnet run --project tools/performance/BaselineGenerator -- --all --output tests/indicators/
```

**Output location**: Files are automatically colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)

### Test scenarios

**Unit Tests (BaselineComparer)**:

- Compare identical results → IsMatch = true, Mismatches empty
- Compare results with any difference → IsMatch = false, Mismatches populated (drift detected)
- Handle null values during warmup period → Compare nulls as equal
- Handle missing properties → Report as mismatch (drift detected)
- Handle timestamp misalignment → Report missing/extra dates (drift detected)

**Integration Tests (BaselineGenerator)**:

- Generate baseline for single indicator → Valid JSON file created, colocated with tests
- Generate baselines for all indicators → All JSON files created (200+) in correct test folders
- Regenerate existing baseline → File overwritten (no metadata timestamp to update)
- Handle missing test data → Graceful error message
- Validate JSON deserialization → Can deserialize to `List<TResult>` successfully
- Deterministic output → Multiple runs produce identical JSON (bit-for-bit)

**Regression Tests (RegressionTestSuite)**:

- Load baseline, run indicator, compare outputs → Test passes
- **Drift introduced** → Test fails immediately with clear diagnostics showing which values changed
- Missing baseline file → Test skipped with warning (not failure)
- Warmup nulls handled → Nulls compared correctly

**Performance Tests**:

- Generate all baselines in < 2 minutes
- Run all regression tests in < 5 minutes
- Parallel baseline generation scales linearly with CPU cores

### Implementation guidelines

**Baseline file format** (simplified per PR #1496):

- Use System.Text.Json with JsonSerializerOptions configured for:
  - PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  - WriteIndented = true (for human readability)
  - DefaultIgnoreCondition = JsonIgnoreCondition.Never (serialize nulls)
  - Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping (minimal escaping)
- **No custom models**: Deserialize directly to `List<TResult>` where TResult is indicator's result class
- Timestamp format: ISO 8601 (`yyyy-MM-ddTHH:mm:ss`) for consistency
- **No metadata wrapper**: Just array of result objects with timestamp and indicator properties

**Numeric comparison logic**:

- Exact binary equality: `expected != actual` determines mismatch (zero tolerance)
- Null handling: null == null → match, null != value → mismatch
- Mathematical precision is NON-NEGOTIABLE per Constitution Principle I
- **Terminology**: Mismatch = technical detection of any difference; Drift = unacceptable deviation requiring investigation and correction (all mismatches constitute drift in this framework)

**Generator tool design** (simplified per PR #1496):

- Leverage existing test data: `TestData.GetDefault()` for Standard scenarios
- Execute indicator method: Invoke `quotes.ToIndicator()` to generate outputs
- **Serialize directly using System.Text.Json** (no custom serialization utilities)
- Output location: Colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
- Parallel generation: Use `Parallel.ForEach` for `--all` mode
- CLI argument parsing: Simple args array or System.CommandLine

**Regression test integration**:

- One test method per indicator: `[TestMethod] public void Sma_Standard_RegressionTest()`
- Test discovery via reflection: Enumerate all indicators from catalog or namespace
- **Baseline loading**: Deserialize directly to `List<TResult>` using System.Text.Json with `TestData.GetDefault()` as source
- Test execution: Run indicator with Standard test data (e.g., `quotes.ToSma(20)` with parameters defined per indicator type)
- Comparison: Use BaselineComparer with exact binary equality
- **Drift diagnostics**: Assert with custom message listing all mismatches (property, timestamp, expected, actual, delta)
- Missing baseline handling: `Assert.Inconclusive("Baseline file not found: {indicatorName}.Baseline.json")`
- **Quality validation**: Each test implementation MUST pass `checklists/regression-test.md` checklist (103 requirement validation items across 10 quality dimensions) before PR approval. Checklist enforces Constitution Principle I alignment, requirement completeness, and consistent implementation patterns.

### Documentation updates

- Add baseline regeneration procedure to `contributing.md` (when to regenerate, how to run generator)
- Add regression testing section to `README.md` (what baselines are, how they protect against drift)
- **Clarify drift terminology**: Drift = unacceptable deviation requiring investigation/correction
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
  3. **Comparer tasks**: Exact binary equality logic, mismatch detection
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

**Estimated output**: 39 tasks (infrastructure + generator + comparer + tests + CI + docs, including validation subtasks T003a, T008a, T016a for TDD split, and T025a for cross-platform validation)

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
