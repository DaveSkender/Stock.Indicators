# .NET source code development

This folder contains the Stock Indicators library source code.

## Quick reference

- Follow .NET development instructions in .github/instructions/dotnet.instructions.md
- Use `decimal` instead of `double` for financial calculations
- Validate all public method parameters
- Document all public APIs with XML comments
- Write unit tests for all code paths

## Common pitfalls to avoid

- **Off-by-one errors**: Double-check lookback period calculations
- **Null reference exceptions**: Validate data before access
- **Precision loss**: Use `decimal` for financial data
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
