# .NET source code development

This folder contains the Stock Indicators library source code.

## Quick reference

- See `.github/skills/` for indicator development guidance (Series, Buffer, Stream)
- **Numerical precision**: This library prioritizes performance while maintaining mathematical accuracy:
  - Public API quote inputs use `decimal` for user convenience (Open, High, Low, Close, Volume)
  - Internal calculations use non-nullable `double` for performance (via `QuoteD` converters)
  - Result types use `double` internally with `.NaN2Null()` conversion at API boundaries
  - Use `double.NaN` for undefined/uncalculable values; never use sentinel values like `0` or `-1`
  - See the NaN handling policy in the root `AGENTS.md` for complete guidance
- Validate all public method parameters
- Document all public APIs with XML comments
- Write unit tests for all code paths

## Common pitfalls to avoid

- **Off-by-one errors**: Double-check lookback period calculations
- **Null reference exceptions**: Validate data before access
- **Type conversions**: Quote inputs are `decimal` → convert to `double` internally → results use `double` with NaN for undefined values
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
