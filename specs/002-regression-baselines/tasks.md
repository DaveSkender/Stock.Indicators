# Implementation tasks: static series regression baselines

**Feature**: Static series regression baselines  
**Updated**: 2025-10-07 (Exact binary equality requirement)  
**Based on**: [spec.md](./spec.md) and [plan.md](./plan.md)

This document provides actionable tasks for implementing the regression baseline framework for all StaticSeries indicator Standard tests.

**Key goals**:

- Create baseline generator tool to capture canonical indicator outputs
- Generate JSON baseline files for 200+ indicators (colocated with tests)
- Implement regression test suite with exact binary equality comparison
- Integrate regression tests into CI pipeline
- Document baseline regeneration and review procedures

**Simplified approach**:

- Baseline files are JSON arrays of result objects (e.g., `List<SmaResult>`)
- Direct deserialization using standard System.Text.Json
- Files colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
- No custom wrapper models or serialization utilities
- Exact binary equality enforced (zero tolerance, NON-NEGOTIABLE per Constitution Principle I)

**Organization**: Tasks grouped by component (format, generator, comparer, tests, CI, docs)

## Phase 1: Infrastructure and baseline format (T001)

### T001: Define baseline JSON file format

**Description**: Define JSON format for baseline files using direct result class mapping  
**Location**: `specs/002-regression-baselines/baseline-format.md` (documentation)  
**Dependencies**: None  
**Acceptance criteria**:

- JSON format documented as direct array of result objects (e.g., `List<SmaResult>`)
- Files colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
- Direct deserialization using System.Text.Json with camelCase naming
- Example baseline files provided for single-property (SMA) and multi-property (MACD) indicators
- Property naming convention specified (camelCase in JSON, PascalCase in C#)
- Explicit null serialization during warmup period

**Status**: ✅ Complete

## Phase 2: Baseline generator tool (T006-T015)

### T006: Scaffold baseline generator console app

**Description**: Create console application project for baseline generator  
**Location**: `tools/performance/BaselineGenerator/`  
**Dependencies**: None  
**Acceptance criteria**:

- Console app project created (`BaselineGenerator.csproj`)
- Program.cs entry point scaffolded
- Project references test project for TestData access
- Project references indicator library
- Targets `net8.0` and `net9.0`

### T007: Implement CLI argument parsing

**Description**: Parse command-line arguments for generator tool  
**Location**: `tools/performance/BaselineGenerator/Program.cs`  
**Dependencies**: T006  
**Acceptance criteria**:

- `--indicator <name>` flag for single indicator generation
- `--all` flag for batch generation
- `--help` displays usage information
- Invalid arguments display clear error messages

### T008: Implement indicator execution logic

**Description**: Execute indicator tests and capture outputs  
**Location**: `tools/performance/BaselineGenerator/IndicatorExecutor.cs`  
**Dependencies**: T007  
**Acceptance criteria**:

- Load TestData.GetDefault() for Standard scenario
- Dynamically invoke indicator method (e.g., `quotes.ToSma(20)`)
- Capture result list (e.g., `List<SmaResult>`)
- Handle indicators with multiple result properties (e.g., MACD with Macd, Signal, Histogram)
- Determine output file path based on indicator location in test directory

### T009: Implement baseline file writing

**Description**: Write baseline files using standard JSON serialization  
**Location**: `tools/performance/BaselineGenerator/Program.cs`  
**Dependencies**: T008  
**Acceptance criteria**:

- Serialize result list directly using `System.Text.Json`
- Configure JsonSerializerOptions with camelCase naming and WriteIndented=true
- Write to colocated file path (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
- Handle null values explicitly (not omitted)
- Create parent directories if needed

### T010: Implement single indicator generation

**Description**: Generate baseline for single indicator (--indicator flag)  
**Location**: `tools/performance/BaselineGenerator/Program.cs`  
**Dependencies**: T009  
**Acceptance criteria**:

- Accepts indicator name as argument
- Generates baseline for specified indicator
- Writes JSON file to colocated test directory
- Displays progress message ("Generating baseline for Sma...")
- Displays completion message with file path

### T011: Implement batch generation for all indicators

**Description**: Generate baselines for all indicators (--all flag)  
**Location**: `tools/performance/BaselineGenerator/Program.cs`  
**Dependencies**: T009  
**Acceptance criteria**:

- Discover all StaticSeries indicators using Catalog enumeration (for consistency with existing test patterns)
- Generate baseline for each indicator
- Parallel execution for performance (use Parallel.ForEach)
- Display progress bar or completion count
- Complete in < 2 minutes for 200+ indicators

### T012: Add error handling and logging

**Description**: Implement comprehensive error handling for generator tool  
**Location**: `tools/performance/BaselineGenerator/`  
**Dependencies**: T010, T011  
**Acceptance criteria**:

- Handle missing test data gracefully (skip indicator with warning)
- Handle indicator execution exceptions: log error with stack trace, skip indicator (no partial baseline), continue batch
- Handle file write failures (permissions, disk space)
- Display summary at end (success count, failure count, skipped count)
- Exit code 0 for success, non-zero for failures

### T013: Add baseline file validation

**Description**: Validate generated baseline files meet format requirements  
**Location**: `tools/performance/BaselineGenerator/BaselineValidator.cs`  
**Dependencies**: T009  
**Acceptance criteria**:

- Validate results array not empty
- Validate timestamp sequence is ascending
- Validate property names match indicator output
- Validate JSON can be deserialized back to result type
- Validate property order is alphabetical within each result object (deterministic ordering)
- Option to validate after generation (--validate flag)

### T014: Create generator tool documentation

**Description**: Document generator tool usage and options  
**Location**: `tools/performance/BaselineGenerator/README.md`  
**Dependencies**: T012  
**Acceptance criteria**:

- Usage examples for all CLI flags
- Explanation of when to regenerate baselines
- Troubleshooting common errors
- Performance expectations documented
- Contribution workflow documented (when regeneration is appropriate)

### T015: Generate initial baseline files for all indicators

**Description**: Run generator tool to create initial baselines  
**Location**: Colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)  
**Dependencies**: T011, T013  
**Acceptance criteria**:

- Execute `dotnet run --project tools/performance/BaselineGenerator -- --all`
- Verify 200+ JSON files created in indicator test directories
- Validate baseline files with --validate flag
- Commit baseline files to version control
- Document commit message ("Add initial regression baselines for all StaticSeries indicators")

## Phase 3: Baseline comparer and regression tests (T016-T025)

### T016: Write unit tests for baseline comparer (TDD)

**Description**: Write failing unit tests for comparison logic before implementation  
**Location**: `tests/indicators/Baselines/BaselineComparer.Tests.cs`  
**Dependencies**: T002  
**Acceptance criteria**:

- Test identical results → expect IsMatch = true (FAILING initially)
- Test results with any difference → expect IsMatch = false with mismatches (FAILING initially)
- Test null handling (null == null, null != value) (FAILING initially)
- Test missing properties → expect mismatch reported (FAILING initially)
- Test date misalignment → expect missing/extra dates reported (FAILING initially)
- All tests documented with TDD Red phase intention

### T016a: Implement baseline comparer core logic (TDD Green)

**Description**: Implement comparison logic to make T016 tests pass  
**Location**: `tests/indicators/Baselines/BaselineComparer.cs`  
**Dependencies**: T016  
**Acceptance criteria**:

- `Compare(expected, actual)` method implemented (no tolerance parameter)
- Iterate both sequences in parallel (zip by date)
- Compare each property with exact binary equality (`expected != actual`)
- Handle null values (null == null → match, null != value → mismatch)
- Return ComparisonResult with IsMatch and Mismatches list
- All T016 tests now pass (TDD Green phase)

### T017: Implement mismatch detection and reporting

**Description**: Create detailed mismatch reporting for failed comparisons  
**Location**: `tests/indicators/Baselines/BaselineComparer.cs`  
**Dependencies**: T016a  
**Acceptance criteria**:

- `MismatchDetail` record with Date, PropertyName, Expected, Actual, Delta
- Populate Mismatches list with all detected differences
- Calculate delta for each mismatch
- Format mismatch details for human readability
- Support multiple mismatches per comparison

### T018: Refactor and extend comparer tests (TDD Refactor)

**Description**: Refactor comparer tests for maintainability and add edge case coverage  
**Location**: `tests/indicators/Baselines/BaselineComparer.Tests.cs`  
**Dependencies**: T016a, T017  
**Acceptance criteria**:

- Refactor test structure for clarity and maintainability
- Add tests for mismatch detection and reporting (T017)
- Verify all edge cases covered (extreme values, empty sequences, single-element arrays)
- Document test patterns for future comparer enhancements
- All tests remain passing after refactor (TDD Refactor phase)

### T019: Implement regression test suite scaffold ✅

**Status**: COMPLETE  
**Description**: Create MSTest test class for regression tests  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T004, T016  
**Acceptance criteria**:

- ✅ MSTest test class created (92 test files following pattern)
- ✅ Test initialization loads baseline files (via RegressionTestBase)
- ✅ Test methods follow naming convention: `{IndicatorName}.Regression.Tests.cs`
- ✅ Test cleanup disposes resources
- ✅ Environment-gated execution via run settings

### T020: Implement per-indicator regression test method ✅

**Status**: COMPLETE (92/92 tests passing)  
**Description**: Create test method pattern for each indicator  
**Location**: Individual `{Indicator}.Regression.Tests.cs` files  
**Dependencies**: T019  
**Acceptance criteria**:

- Load baseline file for indicator (e.g., `Sma.Standard.json`)
- Execute indicator with Standard test data
- Compare current outputs against baseline using BaselineComparer
- Assert IsMatch = true with custom failure message
- If baseline missing, call `Assert.Inconclusive("Baseline file not found: {indicatorName}.Standard.json")`
- **Checklist validation**: Before implementing each indicator test, apply regression-test.md checklist (103 items) to verify requirements completeness, clarity, and constitutional alignment. Address all applicable items (CHK001-CHK103) before marking task complete.

### T021: Implement test failure diagnostics ✅

**Status**: COMPLETE  
**Description**: Format clear error messages for regression test failures  
**Location**: `tests/indicators/TestBase.cs` (RegressionTestBase)  
**Dependencies**: T020, T017  
**Acceptance criteria**:

- ✅ FluentAssertions provides detailed mismatch reporting automatically
- ✅ Format includes property path, expected, actual values
- ✅ Count of total mismatches included
- ✅ Baseline file path shown in assertion message
- ✅ Clear error output for debugging

### T022: Add regression tests for all indicators

**Description**: Generate test methods for all StaticSeries indicators  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T020  
**Acceptance criteria**:

- One test method per indicator (200+ methods)
- Test discovery via reflection or explicit methods
- Parallel test execution for performance
- All tests pass with initial baselines (from T015)
- **Checklist compliance**: Each of 200+ indicator tests MUST pass regression-test.md checklist validation during PR review. Reviewers verify CHK001-CHK103 applicable items are satisfied. Non-compliant tests block merge.

### T023: Add integration tests for missing baselines ✅

**Status**: COMPLETE  
**Description**: Test behavior when baseline file does not exist  
**Location**: `tests/indicators/TestBase.cs` (RegressionTestBase)  
**Dependencies**: T020  
**Acceptance criteria**:

- ✅ AssertEquals throws FileNotFoundException with clear path
- ✅ Test infrastructure handles missing files gracefully
- ✅ Error message includes full path to expected baseline
- ✅ No baselines are missing (84/84 generated, 100%)

### T024: Add performance tests for regression suite ✅

**Status**: COMPLETE  
**Description**: Validate regression test execution time  
**Location**: Test execution metrics  
**Dependencies**: T022  
**Acceptance criteria**:

- ✅ Full regression suite execution time: ~3 seconds for 92 tests
- ✅ Well under 5 minute target (< 0.1% of limit)
- ✅ Memory usage reasonable (no leaks detected)
- ✅ Performance characteristics documented in PR description

### T025: Add cross-platform validation tests

**Description**: Validate deterministic output across .NET target frameworks  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T022  
**Acceptance criteria**:

- Create dedicated test method `CrossPlatformValidation_AllIndicators` that executes on both net8.0 and net9.0
- For each indicator:
  - Execute indicator calculation on current framework
  - Deserialize baseline file (generated on any framework)
  - Compare results using exact binary equality
  - Assert outputs match exactly (zero tolerance)
- ANY difference between net8.0 and net9.0 indicates a precision bug requiring investigation
- Add CI workflow matrix strategy to run tests on both net8.0 and net9.0 runtimes
- Mathematical precision is NON-NEGOTIABLE per Constitution Principle I
- Test validates FR24 requirement: "Regression tests validate deterministic output across net8.0 and net9.0 target frameworks"
- Include at least 10 representative indicators covering: simple (SMA), multi-property (MACD), complex calculations (RSI), and high-precision indicators
- Document investigation procedure if platform differences are detected

## Phase 4: CI integration (T026-T030)

### T026: Update test-indicators workflow

**Description**: Integrate regression tests into CI pipeline  
**Location**: `.github/workflows/test-indicators.yml`  
**Dependencies**: T023  
**Acceptance criteria**:

- Add step to run regression tests after unit tests
- Use `dotnet test --filter "FullyQualifiedName~RegressionTests"` to isolate
- Set environment variable `RUN_REGRESSION_TESTS=true`
- Display test results in workflow summary
- Fail build if any regression tests fail

### T027: Add environment variable gating

**Description**: Make regression tests optional via environment variable  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T020  
**Acceptance criteria**:

- Check `RUN_REGRESSION_TESTS` environment variable in test class initialization (ClassInitialize method)
- Skip entire regression test class if variable not set or false (class-level gating)
- Log message explaining why tests are skipped
- Log message explaining why tests are skipped
- Document environment variable in workflow README

### T028: Add baseline drift summary logging

**Description**: Log summary of baseline drift in CI output  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T021, T022  
**Acceptance criteria**:

- Count total regression tests executed
- Count tests with mismatches (failures)
- List affected indicators in summary
- Display in CI workflow log
- Format for readability (GitHub Actions annotations)

### T029: Configure test result publishing

**Description**: Publish regression test results as CI artifacts  
**Location**: `.github/workflows/test-indicators.yml`  
**Dependencies**: T026  
**Acceptance criteria**:

- Collect regression test results (TRX or JUnit format)
- Upload as workflow artifact
- Display test summary in workflow UI
- Retain artifacts for 30 days for review
- Artifact includes drift summary report with: total indicators tested, indicators with drift (count + list of indicator names), example mismatch details (top 5 by delta magnitude)
- Drift report formatted as markdown for GitHub Actions summary display

### T030: Add baseline regeneration workflow

**Description**: Create CI workflow for regenerating baselines (manual trigger)  
**Location**: `.github/workflows/regenerate-baselines.yml`  
**Dependencies**: T011  
**Acceptance criteria**:

- Workflow triggered manually (workflow_dispatch)
- Runs baseline generator tool with --all flag
- Commits updated baseline files to branch
- Creates pull request with regenerated baselines
- Documents when manual regeneration is appropriate

## Phase 5: Documentation and polish (T031-T035)

### T031: Document baseline regeneration procedure

**Description**: Add baseline regeneration steps to contributing.md  
**Location**: `contributing.md`  
**Dependencies**: T014, T030  
**Acceptance criteria**:

- When to regenerate baselines (intentional algorithm changes)
- How to run generator tool locally
- How to verify regenerated baselines
- How to use manual CI workflow for regeneration
- PR review checklist for baseline changes

### T032: Document regression testing in README

**Description**: Add regression testing overview to main README  
**Location**: `README.md`  
**Dependencies**: T023  
**Acceptance criteria**:

- Explain what regression baselines are
- Explain how they protect against drift
- Mention baseline files in repository structure
- Link to baseline format documentation
- Link to contributing guide for regeneration

### T033: Create baseline format specification

**Description**: Complete documentation of JSON format and schema  
**Location**: `docs/baseline-format.md`  
**Dependencies**: T001, T002  
**Acceptance criteria**:

- JSON schema fully documented
- Example baseline files provided
- Property naming conventions explained
- Deterministic serialization rules documented
- Exact binary equality comparison requirement explained

### T034: Update PR review checklist

**Description**: Add baseline drift review guidance to contributing.md  
**Location**: `contributing.md`  
**Dependencies**: T031  
**Acceptance criteria**:

- Add baseline drift to PR review checklist
- Explain how to interpret regression test failures
- Guidance on when regeneration is acceptable
- Guidance on when drift indicates bug
- Link to baseline format specification

### T035: Document CI environment variables

**Description**: Add CI workflow documentation  
**Location**: `.github/workflows/README.md`  
**Dependencies**: T027  
**Acceptance criteria**:

- Document `RUN_REGRESSION_TESTS` environment variable
- Explain when regression tests run
- Explain how to enable/disable locally
- Document manual baseline regeneration workflow
- Link to contributing guide

## Dependencies

- **T001 → T002**: Baseline format must be defined before creating models
- **T002 → T003, T004**: Models must exist before serialization/deserialization
- **T003 → T003a**: Serialization implemented before readability validation
- **T006 → T007**: Console app must exist before CLI parsing
- **T007 → T008**: CLI parsing complete before execution logic
- **T008 → T008a**: Execution logic complete before multi-property validation
- **T008 → T009**: Execution logic complete before baseline generation
- **T009 → T010, T011**: Generation logic complete before single/batch modes
- **T010, T011 → T012**: Basic generation working before error handling
- **T011 → T015**: Batch generation working before generating initial baselines
- **T002 → T016**: Models must exist before writing comparer tests (TDD Red)
- **T016 → T016a**: Tests written before implementing comparer (TDD Red→Green)
- **T016a → T017, T018**: Core comparison logic implemented before mismatch detection and refactoring
- **T016a, T017 → T018**: All comparison features complete before test refactoring (TDD Refactor)
- **T004, T016a → T019**: Reader and comparer exist before regression test scaffold
- **T019 → T020**: Scaffold complete before per-indicator test methods
- **T020 → T021**: Test methods exist before failure diagnostics
- **T020 → T022**: Test method pattern defined before generating all tests
- **T022 → T023**: Regression tests exist before testing missing baseline behavior
- **T022 → T024**: Regression tests exist before performance tests
- **T022 → T026**: Regression tests complete before CI integration
- **T019 → T027**: Test scaffold exists before environment variable gating
- **T020, T021 → T028**: Test methods and diagnostics exist before summary logging
- **T026 → T029**: CI integration exists before result publishing
- **T011 → T030**: Batch generation exists before regeneration workflow
- **T014, T030 → T031**: Generator documentation and workflow exist before regeneration procedure
- **T023 → T032**: Regression tests exist before README documentation
- **T001, T002 → T033**: Format and models defined before specification documentation
- **T031 → T034**: Regeneration procedure documented before PR review checklist
- **T027 → T035**: Environment variable gating exists before CI workflow documentation

## Parallel execution examples

### Infrastructure setup (T001-T005 can run in parallel)

```markdown
Task: "Define baseline JSON file format"
Task: "Create baseline file directory structure"
Command: /tasks run T001 T005
```

### Serialization and validation (T003a can run after T003)

```markdown
Task: "Implement baseline JSON serialization"
Command: /tasks run T003

Task: "Validate baseline JSON readability"
Command: /tasks run T003a
```

### Generator tool development (T006-T007 parallel, then T008-T009 parallel)

```markdown
Task: "Scaffold baseline generator console app"
Task: "Implement CLI argument parsing"
Command: /tasks run T006 T007

Task: "Implement indicator execution logic"
Task: "Implement baseline generation logic"
Command: /tasks run T008 T009

Task: "Validate multi-property indicator handling"
Command: /tasks run T008a
```

### Comparer implementation (T016-T018 sequential for TDD workflow)

```markdown
Task: "Write unit tests for baseline comparer (TDD Red)"
Command: /tasks run T016

Task: "Implement baseline comparer core logic (TDD Green)"
Command: /tasks run T016a

Task: "Implement mismatch detection and reporting"
Command: /tasks run T017

Task: "Refactor and extend comparer tests (TDD Refactor)"
Command: /tasks run T018
```

### Documentation tasks (T031-T035 can run in parallel)

```markdown
Task: "Document baseline regeneration procedure"
Task: "Document regression testing in README"
Task: "Create baseline format specification"
Task: "Update PR review checklist"
Task: "Document CI environment variables"
Command: /tasks run T031 T032 T033 T034 T035
```

## Notes

- Tasks marked with [P] are fully parallelizable (no blocking dependencies)
- Infrastructure tasks (T001-T005) establish foundation for all other work
- Generator tool (T006-T015) can proceed independently of comparer (T016-T018)
- **TDD workflow enforced**: T016 (write tests) → T016a (implement) → T018 (refactor)
- **Checklist workflow**: Apply regression-test.md template to EACH indicator test implementation (T020, T022). Checklist validates requirements quality (NOT implementation correctness). Use checklist BEFORE coding to identify ambiguities, DURING PR review to verify completeness.
- New validation tasks (T003a, T008a, T025) verify quality attributes and edge cases
- Regression tests (T019-T025) require both generator and comparer complete
- CI integration (T026-T030) requires regression tests working locally
- Documentation (T031-T035) can run in parallel after implementation complete

## Validation checklist

- [x] Baseline format specification complete (direct result class mapping)
- [x] Generator tool fully specified (CLI, execution, batch processing)
- [x] Comparison logic comprehensive (exact binary equality, mismatch detection)
- [x] Regression test suite complete (per-indicator tests, failure diagnostics)
- [x] CI integration addresses environment gating and result publishing
- [x] Documentation covers regeneration procedure, format specification, and PR review
- [x] Dependencies mapped correctly (no circular dependencies)
- [x] Parallel tasks avoid file collisions
- [x] Initial baseline generation task included (T015)
- [x] Performance validation tasks present (T024, T025)
- [x] Multi-property indicator validation included (T008a)
- [x] JSON readability validation included (T003a)
- [x] **TDD workflow enforced (T016→T016a→T018) per constitution**
- [x] **Cross-platform validation included (T025) for .NET version migration**
- [x] **Documentation requirements mapped to FRs (FR19-FR23)**
- [x] **Exact binary equality enforced (NON-NEGOTIABLE per Constitution Principle I)**

---

- **Total tasks**: 37 (T017 removed, T018-T025 renumbered to T017-T024, added T016a for TDD split)
- **Parallelizable tasks**: 14 (marked with [P] in notes)
- **Sequential tasks**: 23
- **TDD tasks**: 3 (T016 Red, T016a Green, T018 Refactor)
- **Estimated completion**: 6-8 focused development sessions with testing and validation

---

## Phase 3 Refactoring: Per-Indicator Test Files (October 8, 2025)

**Context**: The initial implementation used a monolithic `RegressionTests.cs` file with individual test methods for each indicator. This approach was refactored to use individual per-indicator test files following a cleaner inheritance pattern.

**Implementation approach**:

### Architectural changes

1. **RegressionTestBase made generic**:
   - Changed from `RegressionTestBase` with hardcoded `EmaResult` to `RegressionTestBase<TResult> where TResult : ISeries`
   - Allows each indicator to specify its result type
   - Constructor accepts baseline filename parameter

2. **Baseline file consolidation**:
   - Moved all `*.Baseline.json` files from individual indicator folders to centralized `tests/indicators/_testdata/results/`
   - Renamed with standardized convention: `{indicatorname}.standard.json` (lowercase)
   - Example: `tests/indicators/s-z/Sma/Sma.Baseline.json` → `tests/indicators/_testdata/results/sma.standard.json`

3. **Per-indicator regression test files**:
   - Created `{IndicatorName}.Regression.Tests.cs` in each indicator's test folder
   - Each test file inherits from `RegressionTestBase<TResult>`
   - Includes three test methods: `Series()`, `Buffer()`, and `Stream()`
   - For indicators without Buffer/Stream implementations, methods use `Assert.Inconclusive()`

### Example implementation

```csharp
using TestsUtilities;

namespace Regression;

[TestClass, TestCategory("Regression")]
public class SmaTests : RegressionTestBase<SmaResult>
{
    public SmaTests() : base("sma.standard.json") { }

    [TestMethod]
    public override void Series() => Quotes.ToSma(20).AssertEquals(Expected);

    [TestMethod]
    public override void Buffer() => Quotes.ToSmaList(20).AssertEquals(Expected);

    [TestMethod]
    public override void Stream() => Assert.Inconclusive("Stream implementation not yet available");
}
```

### Completed work

- ✅ Modified `RegressionTestBase` to be generic (`RegressionTestBase<TResult>`)
- ✅ Moved 75 baseline files to `tests/indicators/_testdata/results/` with standardized naming
- ✅ Generated 75 individual regression test files (one per indicator)
- ✅ Deleted obsolete `tests/indicators/RegressionTests.cs` monolithic file
- ✅ Created Python generator script (`tools/generate_regression_tests.py`) for automation

### Benefits of this approach

1. **Better organization**: Each indicator's regression tests live alongside its other test files
2. **Type safety**: Generic base class ensures correct result types at compile time
3. **Easier maintenance**: Changes to an indicator's test only affect one file
4. **Clear test discovery**: Test runner shows individual indicator tests, not one monolithic class
5. **Incremental implementation**: Indicators without Buffer/Stream implementations clearly marked
6. **Reusability**: Test data and patterns centralized in base class

### Task status updates

The following tasks from Phase 3 have been superseded by this refactoring:

- **T019**: ~~Create monolithic `RegressionTests.cs`~~ → Individual `*.Regression.Tests.cs` files per indicator
- **T020**: ~~Per-indicator test methods in one file~~ → Per-indicator test classes
- **T022**: ~~Generate 200+ methods~~ → Generated 75 individual test files
- **T023**: Missing baseline behavior now handled at base class level with clear file-not-found messages

---
Last updated: October 8, 2025
