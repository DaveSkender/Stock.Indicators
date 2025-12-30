# Test suite

This folder contains unit tests, integration tests, and performance benchmarks.

## Test organization

- `indicators/`: Unit tests for all indicators
- `other/`: Integration and utility tests
- `performance/`: Performance benchmarks

## Running tests

Use `.runsettings` files for test isolation:

```bash
# Unit tests only (excludes Integration and Regression)
dotnet test tests/indicators/ --settings tests/tests.unit.runsettings

# Regression tests only (baseline validation)
dotnet test tests/indicators/ --settings tests/tests.regression.runsettings

# Integration tests only (requires external dependencies)
dotnet test tests/integration/ --settings tests/tests.integration.runsettings

# All tests (no filter)
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test categories

- **Unit tests**: Fast, isolated indicator calculations (default for development)
- **Regression tests**: Validate against baseline JSON files (run before releases)
- **Integration tests**: External dependencies, API contracts, public API surface

## Writing tests

- Write tests for all public methods
- Cover edge cases: empty input, minimum/maximum values, boundary conditions
- Use descriptive test names that explain the scenario
- Keep tests focused on single behaviors
- Maintain baseline data for indicator validation

---
Last updated: December 30, 2025
