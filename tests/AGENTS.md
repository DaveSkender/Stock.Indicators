# Test suite

This folder contains unit tests, integration tests, and performance benchmarks.

## Test organization

- indicators/ - Unit tests for all indicators
- other/ - Integration and utility tests  
- performance/ - Performance benchmarks

## Running tests

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

See .github/skills/testing-standards/SKILL.md for test writing guidance and conventions.

---
Last updated: January 25, 2026
