# Feature specification: static series regression baselines

**Branch**: `plan-regression-tests` | **Date**: 2025-10-06 | **Status**: Draft

## Overview

Implement a comprehensive regression baseline framework for all StaticSeries indicator `Standard` tests to ensure future changes do not unintentionally alter indicator outputs. The framework generates, stores, and validates canonical output datasets using JSON files for reproducibility and drift detection.

## Background

The Stock Indicators library contains 200+ technical indicators with batch-style (StaticSeries) implementations. Each indicator has a `Standard` test scenario using reference market data. Currently, these tests validate mathematical correctness but lack deterministic regression baselines to detect unintended output changes across versions.

This framework establishes "known good" output snapshots that can be regenerated when algorithm changes are intentional, while catching accidental behavioral drift during refactoring, dependency updates, or platform changes.

## User scenarios & testing

### Primary use cases

1. **Continuous integration regression detection**: Automatically detect when indicator outputs drift from established baselines during PR validation
2. **Refactoring confidence**: Enable safe code refactoring with immediate feedback if outputs change unexpectedly
3. **Version migration validation**: Verify indicator behavior remains consistent when upgrading .NET versions or dependencies
4. **Algorithm change documentation**: Provide clear before/after comparison when intentionally modifying indicator calculations

### Success scenarios

- Developer refactors indicator code → CI fails if outputs change → developer investigates or regenerates baselines intentionally
- Library upgrades to .NET 10 → regression tests verify all indicators produce identical results
- New floating-point optimization → regression suite identifies which indicators are affected and by how much
- Team reviews PR → regression test results show zero drift → merge with confidence

## Functional requirements

### Core features

- **FR1**: Generate JSON baseline files containing complete indicator outputs (including warmup nulls) for each StaticSeries indicator's `Standard` test
- **FR2**: Store baselines in version control under `tests/indicators/.baselines/` with one JSON file per indicator
- **FR3**: Compare current test outputs against stored baselines using configurable numeric tolerances
- **FR4**: Report baseline drift with clear diagnostics (which values changed, by how much, at which dates)
- **FR5**: Support strict mode for zero-tolerance comparison (e.g., after intentional algorithm changes)

### Baseline generation

- **FR6**: Baseline generator tool (`tools/performance/BaselineGenerator/`) creates JSON files from current test execution
- **FR7**: Generator captures all result properties for each date; null values during warmup period MUST be serialized explicitly (not omitted) for complete output representation
- **FR8**: Metadata includes indicator name, test scenario, generation timestamp, and library version
- **FR9**: JSON format uses consistent serialization (camelCase, minimal whitespace, deterministic property order)
- **FR10**: Generator supports regenerating single indicator or all indicators in batch

### Regression validation

- **FR11**: Regression test suite in `tests/indicators/` compares current outputs against baselines
- **FR12**: Numeric comparison uses configurable tolerance (default: 1e-12 for floating-point precision)
- **FR13**: Strict mode enforces exact match (tolerance = 0) for critical validations
- **FR14**: Test failures report file path, property name, date, expected value, actual value, and delta
- **FR15**: Tests use `Assert.Inconclusive()` when baseline file does not exist (warn instead of fail) with message format: `"Baseline file not found: {indicatorName}.Standard.json"`

### CI integration

- **FR16**: Regression tests run automatically in CI pipeline alongside existing unit tests
- **FR17**: CI workflow includes environment variable to enable/disable regression gating
- **FR18**: CI logs baseline drift summary (count of mismatches, affected indicators)
- **FR19**: CI artifacts include detailed regression report for post-merge review

## Key entities

- **BaselineFile**: JSON file containing complete indicator output for one Standard test (metadata + results array)
- **BaselineMetadata**: Indicator name, test scenario name, generation timestamp, library version, warmup period count
- **BaselineResult**: Single result entry with date and properties (dictionary mapping property name to nullable double value)
- **BaselineComparer**: Logic to compare two result sequences with tolerance and strict mode
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

- Implement `BaselineComparer` with numeric tolerance and strict mode
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

- Baseline files assume deterministic floating-point behavior across .NET versions (verified via tolerance)
- JSON format prioritizes human readability for code review over minimal file size
- Baseline files are version-controlled (not generated dynamically at test time)
- Generator tool runs on same test data as existing `Standard` tests (no new test data required)
- Numeric tolerance accounts for acceptable floating-point precision differences

## Dependencies

- Existing StaticSeries indicator implementations and Standard tests
- MSTest framework for regression test suite
- System.Text.Json for baseline serialization
- CI infrastructure (GitHub Actions workflows)

## Risks & mitigation

- **Floating-point drift across platforms**: Mitigated by configurable tolerance and strict mode for validating fixes
- **Baseline file merge conflicts**: Mitigated by one file per indicator and clear regeneration process
- **False positives from acceptable changes**: Mitigated by tolerance configuration and baseline regeneration workflow
- **CI performance impact**: Mitigated by parallel test execution and selective regression gating via environment variable

---
Last updated: October 6, 2025
