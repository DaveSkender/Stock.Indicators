# Regression test baselines

This directory contains JSON baseline files for regression testing of StaticSeries indicators.

## Purpose

Each baseline file captures the canonical output of an indicator's Standard test scenario. Regression tests compare current indicator outputs against these baselines to detect unintended behavioral drift.

## File format

- **Naming**: `{IndicatorName}.Standard.json` (e.g., `Sma.Standard.json`)
- **Content**: JSON with metadata and results array
- **Specification**: See `specs/002-regression-baselines/baseline-format.md`

## When to regenerate

Baselines should be regenerated when:

- Making intentional changes to indicator algorithms
- Upgrading to a new .NET version
- Modifying test data
- Initial baseline creation for new indicators

## How to regenerate

```bash
# Regenerate a single indicator baseline
dotnet run --project tools/performance/BaselineGenerator -- --indicator Sma

# Regenerate all baselines
dotnet run --project tools/performance/BaselineGenerator -- --all

# Specify custom output directory
dotnet run --project tools/performance/BaselineGenerator -- --all --output tests/indicators/.baselines/
```

## Reviewing baseline changes

When baseline files change in a PR:

1. Check metadata fields (version, warmup count)
2. Verify numeric changes are expected and documented
3. Confirm property names match indicator properties
4. Validate that null patterns match warmup period

## Common issues

- **Many decimal places**: Full precision is preserved by design
- **Date alignment**: Dates must match test data exactly
- **Property case**: Must follow camelCase convention
- **Missing nulls**: Warmup nulls must be explicitly present

## Version control

These baseline files **ARE** version-controlled. Do not add `.baselines/` to `.gitignore`.

---
Last updated: October 6, 2025
