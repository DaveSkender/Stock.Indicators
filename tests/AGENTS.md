# Test suite

This folder contains unit tests, integration tests, and performance benchmarks.

## Test organization

- `indicators/` — unit tests for all indicators
- `other/` — integration and utility tests
- `performance/` — performance benchmarks

## Commands

```bash
# Unit tests only
dotnet test tests/indicators/ --settings tests/tests.unit.runsettings

# Regression tests only
dotnet test tests/indicators/ --settings tests/tests.regression.runsettings

# Integration tests only
dotnet test tests/integration/ --settings tests/tests.integration.runsettings

# All tests
dotnet test
```

Load #skill:testing-standards for test naming conventions, FluentAssertions patterns, precision requirements, and test base class selection.

## Boundaries

✅ Always write tests before marking an indicator implementation complete

✅ Always verify Stream/Buffer results match Series results for the same inputs

⚠️ Ask before changing baseline test data — run the baseline generation task and review the diff

🚫 Never skip or exclude a failing test without fixing the root cause

🚫 Never add `[Ignore]` attributes without a tracked issue and justification
