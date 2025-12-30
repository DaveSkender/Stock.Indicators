# Test suite

This folder contains unit tests, integration tests, and performance benchmarks.

## Test organization

- `indicators/`: Unit tests for all indicators
- `other/`: Integration and utility tests
- `performance/`: Performance benchmarks

## Running tests

```bash
# Run all tests from solution root
dotnet test

# Run specific test project
dotnet test tests/indicators/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Writing tests

- Write tests for all public methods
- Cover edge cases: empty input, minimum/maximum values, boundary conditions
- Use descriptive test names that explain the scenario
- Keep tests focused on single behaviors
- Maintain baseline data for indicator validation

---
Last updated: December 30, 2025
