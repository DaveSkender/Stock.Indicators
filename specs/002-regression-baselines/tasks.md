# Implementation tasks: static series regression baselines

**Feature**: Static series regression baselines  
**Updated**: 2025-01-09 (Simplified approach)  
**Based on**: [spec.md](./spec.md) and [plan.md](./plan.md)

## Task overview

This document provides actionable tasks for implementing the regression baseline framework for all StaticSeries indicator Standard tests.

**Key goals**:

- Create baseline generator tool to capture canonical indicator outputs
- Generate JSON baseline files for 200+ indicators (colocated with tests)
- Implement regression test suite with configurable tolerance
- Integrate regression tests into CI pipeline
- Document baseline regeneration and review procedures

**Simplified approach**:

- Baseline files are JSON arrays of result objects (e.g., `List<SmaResult>`)
- Direct deserialization using standard System.Text.Json
- Files colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
- No custom wrapper models or serialization utilities

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

- Discover all StaticSeries indicators from catalog or namespace
- Generate baseline for each indicator in colocated test directory
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

### T016: Implement baseline comparer core logic

**Description**: Create comparison logic with tolerance support  
**Location**: `tests/indicators/Baselines/BaselineComparer.cs`  
**Dependencies**: T002  
**Acceptance criteria**:

- `Compare(expected, actual, tolerance)` method
- Iterate both sequences in parallel (zip by date)
- Compare each property with tolerance (`Math.Abs(expected - actual) > tolerance`)
- Handle null values (null == null → match, null != value → mismatch)
- Return ComparisonResult with IsMatch and Mismatches list

### T017: Implement mismatch detection and reporting

**Description**: Create detailed mismatch reporting for failed comparisons  
**Location**: `tests/indicators/Baselines/BaselineComparer.cs`  
**Dependencies**: T016  
**Acceptance criteria**:

- `MismatchDetail` record with Date, PropertyName, Expected, Actual, Delta
- Populate Mismatches list with all detected differences
- Calculate delta for each mismatch
- Format mismatch details for human readability
- Support multiple mismatches per comparison

### T018: Implement strict mode comparison

**Description**: Add zero-tolerance comparison for strict validation  
**Location**: `tests/indicators/Baselines/BaselineComparer.cs`  
**Dependencies**: T016  
**Acceptance criteria**:

- `CompareStrict(expected, actual)` method
- Uses tolerance = 0 (exact binary equality)
- Same mismatch reporting as standard comparison
- Clearly documented use case (after intentional algorithm changes)

### T019: Add unit tests for BaselineComparer

**Description**: Comprehensive unit tests for comparison logic  
**Location**: `tests/indicators/Baselines/BaselineComparer.Tests.cs`  
**Dependencies**: T016, T017, T018  
**Acceptance criteria**:

- Test identical results → IsMatch = true
- Test results within tolerance → IsMatch = true
- Test results exceeding tolerance → IsMatch = false with mismatches
- Test strict mode with exact match → IsMatch = true
- Test strict mode with any difference → IsMatch = false
- Test null handling (null == null, null != value)
- Test missing properties → mismatch reported
- Test date misalignment → missing/extra dates reported

### T020: Implement regression test suite scaffold

**Description**: Create MSTest test class for regression tests  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T004, T016  
**Acceptance criteria**:

- MSTest test class created
- Test initialization loads baseline files
- Test methods follow naming convention: `{IndicatorName}_Standard_RegressionTest`
- Test cleanup disposes resources
- Parallel test execution enabled (if safe)

### T021: Implement per-indicator regression test method

**Description**: Create test method pattern for each indicator  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T020  
**Acceptance criteria**:

- Load baseline file for indicator (e.g., `Sma.Standard.json`)
- Execute indicator with Standard test data
- Compare current outputs against baseline using BaselineComparer
- Assert IsMatch = true with custom failure message
- If baseline missing, call `Assert.Inconclusive("Baseline file not found")`

### T022: Implement test failure diagnostics

**Description**: Format clear error messages for regression test failures  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T021, T017  
**Acceptance criteria**:

- List all mismatches in failure message
- Format: "Date: 2016-02-01, Property: sma, Expected: 214.52, Actual: 214.53, Delta: 0.01"
- Include count of total mismatches
- Include baseline file path in message
- Suggest regenerating baseline if intentional change

### T023: Add regression tests for all indicators

**Description**: Generate test methods for all StaticSeries indicators  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T021  
**Acceptance criteria**:

- One test method per indicator (200+ methods)
- Test discovery via reflection or explicit methods
- Parallel test execution for performance
- All tests pass with initial baselines (from T015)

### T024: Add integration tests for missing baselines

**Description**: Test behavior when baseline file does not exist  
**Location**: `tests/indicators/RegressionTests.cs`  
**Dependencies**: T021  
**Acceptance criteria**:

- Temporarily rename baseline file to simulate missing
- Execute regression test
- Verify test result is Inconclusive (not Failed)
- Verify warning message includes missing file path
- Restore baseline file after test

### T025: Add performance tests for regression suite

**Description**: Validate regression test execution time  
**Location**: `tools/performance/Perf.Regression.cs`  
**Dependencies**: T023  
**Acceptance criteria**:

- Benchmark full regression test suite execution
- Verify < 5 minutes for all 200+ indicators
- Measure memory usage during tests
- Document performance characteristics

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

- Check `RUN_REGRESSION_TESTS` environment variable in test initialization
- Skip all regression tests if variable not set or false
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
- Comparison tolerance explained

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
- **T002 → T016**: Models must exist before comparison logic
- **T016 → T017, T018**: Core comparison logic before mismatch detection and strict mode
- **T016, T017, T018 → T019**: All comparison logic complete before unit tests
- **T004, T016 → T020**: Reader and comparer exist before regression test scaffold
- **T020 → T021**: Scaffold complete before per-indicator test methods
- **T021 → T022**: Test methods exist before failure diagnostics
- **T021 → T023**: Test method pattern defined before generating all tests
- **T023 → T024**: Regression tests exist before testing missing baseline behavior
- **T023 → T025**: Regression tests exist before performance tests
- **T023 → T026**: Regression tests complete before CI integration
- **T020 → T027**: Test scaffold exists before environment variable gating
- **T021, T022 → T028**: Test methods and diagnostics exist before summary logging
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

### Comparer implementation (T016-T018 can partially overlap)

```markdown
Task: "Implement baseline comparer core logic"
Task: "Implement mismatch detection and reporting"
Task: "Implement strict mode comparison"
Command: /tasks run T016 T017 T018
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
- Generator tool (T006-T015) can proceed independently of comparer (T016-T019)
- New validation tasks (T003a, T008a) verify quality attributes and edge cases
- Regression tests (T020-T025) require both generator and comparer complete
- CI integration (T026-T030) requires regression tests working locally
- Documentation (T031-T035) can run in parallel after implementation complete

## Validation checklist

- [x] Baseline format specification complete (direct result class mapping)
- [x] Generator tool fully specified (CLI, execution, batch processing)
- [x] Comparison logic comprehensive (tolerance, strict mode, mismatch detection)
- [x] Regression test suite complete (per-indicator tests, failure diagnostics)
- [x] CI integration addresses environment gating and result publishing
- [x] Documentation covers regeneration procedure, format specification, and PR review
- [x] Dependencies mapped correctly (no circular dependencies)
- [x] Parallel tasks avoid file collisions
- [x] Initial baseline generation task included (T015)
- [x] Performance validation tasks present (T025)
- [x] Files colocated with indicator tests (not separate directory)
- [x] Simplified approach using standard JSON serialization

---

- **Total tasks**: 35 (simplified from 37, removed T002-T005, T003a, T008a)
- **Parallelizable tasks**: 12 (marked with [P] in notes)
- **Sequential tasks**: 23
- **Estimated completion**: 5-7 focused development sessions with testing and validation

---
Last updated: January 9, 2025
