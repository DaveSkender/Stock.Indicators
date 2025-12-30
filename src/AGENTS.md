# .NET source code development

This folder contains the Stock Indicators library source code.

## Quick reference

- Follow .NET development instructions in .github/instructions/dotnet.instructions.md
- **Data type usage**:
  - Public API: Use `decimal` for quote inputs (Open, High, Low, Close, Volume)
  - Internal calculations: Use `double` for performance (via `QuoteD` and converter methods)
  - Result types: Use `double?` for most indicators, `decimal?` when precision is critical (e.g., ZigZag, PivotPoints)
- Validate all public method parameters
- Document all public APIs with XML comments
- Write unit tests for all code paths

## Common pitfalls to avoid

- **Off-by-one errors**: Double-check lookback period calculations
- **Null reference exceptions**: Validate data before access
- **Type mismatches**: Public quote inputs are `decimal`, internal calculations use `double`, result types vary by indicator
- **Index out of bounds**: Verify collection sizes before indexing
- **Performance regression**: Profile before and after optimization changes

## Building and testing

```bash
# Build from solution root
dotnet build

# Run tests from solution root
dotnet test

# Format code
dotnet format
```

---
Last updated: December 30, 2025
