# Feature specification: static series regression baselines

**Branch**: `plan-regression-tests` | **Date**: 2025-10-06 | **Status**: Draft

## Overview

Implement a comprehensive regression baseline framework for all StaticSeries indicator `Standard` tests to ensure future changes do not unintentionally alter indicator outputs. The framework generates, stores, and validates canonical output datasets using JSON files for reproducibility and detection of unintended behavioral changes.

### Terminology

This specification uses the following terms consistently:

- **Mismatch**: Any comparison difference detected by the baseline comparer. All mismatches are reported with specific values, deltas, and locations.
- **Drift**: Unacceptable deviation from expected baseline values that requires investigation and correction. Any mismatch constitutes drift and blocks CI merge. Drift must be either fixed (if unintentional) or baselines regenerated (if intentional change).
- **Baseline**: Known-good canonical output stored as JSON for regression comparison.

## Background

The Stock Indicators library contains 200+ technical indicators with batch-style (StaticSeries) implementations. Each indicator has a `Standard` test scenario using reference market data. Currently, these tests validate mathematical correctness but lack deterministic regression baselines to detect unintended output changes across versions.

This framework establishes "known good" output snapshots that can be regenerated when algorithm changes are intentional, while catching accidental behavioral changes during refactoring, dependency updates, or platform changes.

## User scenarios & testing

### Primary use cases

- **Continuous integration regression detection**: Automatically detect when indicator outputs drift (deviate unacceptably) from established baselines during PR validation
- **Refactoring confidence**: Enable safe code refactoring with immediate feedback if outputs change unexpectedly
- **Version migration validation**: Verify indicator behavior remains consistent when upgrading .NET versions or dependencies
- **Algorithm change documentation**: Provide clear before/after comparison when intentionally modifying indicator calculations

### Success scenarios

- Developer refactors indicator code → CI fails if outputs change → developer investigates or regenerates baselines intentionally
- Library upgrades to .NET 10 → regression tests verify all indicators produce identical results
- New floating-point optimization → regression suite identifies which indicators are affected and by how much
- Team reviews PR → regression test results show zero drift → merge with confidence

## Functional requirements

### Core features

- **FR1**: Generate JSON baseline files containing complete indicator outputs (including warmup nulls) for each StaticSeries indicator's `Standard` test
- **FR2**: Store baselines in version control colocated with indicator test files following pattern `tests/indicators/{a-d|e-k|m-r|s-z}/{IndicatorName}/{IndicatorName}.Baseline.json` with one JSON file per indicator (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
- **FR3**: Compare current test outputs against stored baselines using exact binary equality (zero tolerance)
- **FR4**: Report comparison mismatches with clear diagnostics (which values changed, by how much, at which dates); any mismatch constitutes drift requiring investigation and correction

### Baseline generation

- **FR6**: Baseline generator tool (`tools/performance/BaselineGenerator/`) creates JSON files from current test execution
- **FR7**: Generator captures all result properties for each date; null values during warmup period MUST be serialized explicitly (not omitted) for complete output representation
- **FR8**: JSON files contain direct array of result objects without metadata wrapper (simplified approach); each baseline represents the complete output for one indicator's Standard test scenario
- **FR9**: JSON format uses consistent serialization (camelCase, minimal whitespace, deterministic property order)
- **FR10**: Generator supports regenerating single indicator or all indicators in batch

### Regression validation

- **FR11**: Regression test suite in `tests/indicators/` compares current outputs against baselines
- **FR12**: Numeric comparison enforces exact binary equality for all indicator outputs (mathematical precision is NON-NEGOTIABLE per Constitution Principle I)
- **FR13**: Test failures report file path, property name, date, expected value, actual value, and delta for each mismatch
- **FR14**: Tests use `Assert.Inconclusive()` to skip test (not fail) when baseline file does not exist with message format: `"Baseline file not found: {indicatorName}.Standard.json"`

### CI integration

- **FR15**: Regression tests run automatically in CI pipeline alongside existing unit tests
- **FR16**: CI workflow includes environment variable to enable/disable regression gating
- **FR17**: CI logs summary of detected mismatches (count, affected indicators) with drift classification (any mismatch blocks merge)
- **FR18**: CI artifacts include detailed mismatch report for post-merge review

### Documentation requirements

- **FR19**: Baseline format specification document (`specs/002-regression-baselines/baseline-format.md`) describes JSON schema, property conventions, and serialization rules
- **FR20**: Contributor guide (`contributing.md`) includes baseline regeneration procedure (when, why, how)
- **FR21**: Generator tool documentation (`tools/performance/BaselineGenerator/README.md`) covers CLI usage, troubleshooting, and performance expectations
- **FR22**: Pull request review checklist updated with baseline drift review guidance
- **FR23**: CI workflow documentation explains environment variables and regression test execution
- **FR24**: Per-indicator regression test checklist (`checklists/regression-test.md`) validates requirements quality for consistent test implementation across 200+ indicators

### Cross-platform validation

- **FR25**: Regression tests validate deterministic output across net8.0 and net9.0 target frameworks to ensure version migration compatibility

## Key entities

- **BaselineFile**: JSON file containing complete indicator output for one Standard test
- **BaselineResult**: Single result entry with date and properties (dictionary mapping property name to nullable double value)
- **BaselineComparer**: Logic to compare two result sequences using exact binary equality
- **BaselineGenerator tool**: Console application to execute Standard tests and serialize outputs to JSON (located in `tools/performance/BaselineGenerator/`)
- **RegressionTestSuite**: MSTest test class running baseline comparisons for all indicators

## Implementation phases

### Phase 1: Baseline file format and generator tool (Immediate priority)

- Define JSON schema for baseline files (metadata + results array)
- Implement baseline generator tool (console app) in `tools/performance/BaselineGenerator/`
- Generate initial baselines for all StaticSeries indicators using Standard test data
- Store baseline JSON files in `tests/indicators/.baselines/`
- Commit baseline files to version control

### Phase 2: Regression test suite (High priority)

- Implement `BaselineComparer` using exact binary equality
- Create `RegressionTests` test class in `tests/indicators/`
- Implement per-indicator regression test methods comparing current outputs to baselines
- Add clear failure diagnostics (expected vs actual with delta)
- Handle missing baseline files gracefully (warn instead of fail)

### Phase 3: CI integration and workflows (Medium priority)

- Update `test-indicators.yml` workflow to include regression tests
- Add environment variable `RUN_REGRESSION_TESTS` to gate regression execution
- Implement regression drift summary logging
- Configure test result publishing for regression failures

### Phase 4: Documentation and maintenance (Ongoing)

- Document baseline regeneration procedure in `contributing.md`
- Add regression testing section to `README.md`
- Create baseline file format specification in `docs/`
- Provide examples of reviewing regression drift in PR reviews

## Success criteria

- All StaticSeries indicators have baseline JSON files committed to version control
- Regression test suite validates 200+ indicators against baselines with <5 minutes total execution time
- Zero false positives (no baseline drift unless code actually changes outputs)
- CI pipeline catches unintended drift before merge
- Baseline regeneration completes in <2 minutes for all indicators
- Documentation enables any contributor to understand and use regression baselines

## Constraints & assumptions

- Mathematical precision is NON-NEGOTIABLE per Constitution Principle I—exact binary equality required
- JSON format prioritizes human readability for code review over minimal file size
- Baseline files are version-controlled (not generated dynamically at test time)
- Generator tool runs on same test data as existing `Standard` tests (no new test data required)
- Any platform or .NET version differences indicate bugs requiring investigation

## Dependencies

- Existing StaticSeries indicator implementations and Standard tests
- MSTest framework for regression test suite
- System.Text.Json for baseline serialization
- CI infrastructure (GitHub Actions workflows)

## Risks & mitigation

- **Baseline file merge conflicts**: Mitigated by one file per indicator and clear regeneration process
- **CI performance impact**: Mitigated by parallel test execution and selective regression gating via environment variable
- **Platform-specific floating-point differences**: If differences exist between net8.0 and net9.0, this indicates a precision bug in the indicator implementation that must be fixed (not tolerated). Mathematical precision is NON-NEGOTIABLE per Constitution Principle I.

---
Last updated: October 6, 2025
