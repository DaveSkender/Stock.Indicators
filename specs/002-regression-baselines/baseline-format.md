# Baseline JSON file format specification

**Feature**: Static series regression baselines  
**Version**: 1.0  
**Date**: October 6, 2025

## Overview

This document defines the JSON schema for regression baseline files. These files store canonical indicator outputs for comparison against current test results to detect unintended behavioral drift.

## File naming convention

- Pattern: `{IndicatorName}.{ScenarioName}.json`
- Example: `Sma.Standard.json`, `Macd.Standard.json`
- Location: `tests/indicators/.baselines/`
- Case sensitivity: Matches indicator class name (e.g., `Sma`, not `SMA`)

## JSON schema

### Root structure

```json
{
  "metadata": {
    "indicatorName": "string",
    "scenarioName": "string",
    "generatedAt": "ISO8601 datetime",
    "libraryVersion": "string",
    "warmupPeriodCount": number
  },
  "results": [
    {
      "date": "ISO8601 date",
      "propertyName1": number | null,
      "propertyName2": number | null
    }
  ]
}
```

### Metadata fields

- **indicatorName** (required): Indicator name matching the class name (e.g., "Sma", "Macd")
- **scenarioName** (required): Test scenario identifier (e.g., "Standard")
- **generatedAt** (required): UTC timestamp when baseline was generated (ISO 8601 format)
- **libraryVersion** (required): Library version string (e.g., "3.0.0")
- **warmupPeriodCount** (required): Number of periods required for indicator warmup

### Results array

Each result object represents one date's calculation:

- **date** (required): Date in ISO 8601 format (e.g., "2016-01-04")
- **property fields** (variable): One or more indicator result properties
  - Property names use camelCase convention (e.g., "sma", "macd", "signal")
  - Values are either `number` (double precision) or `null` (during warmup)
  - Null values MUST be explicitly serialized (not omitted)

## Example baseline files

### Single-property indicator (SMA)

```json
{
  "metadata": {
    "indicatorName": "Sma",
    "scenarioName": "Standard",
    "generatedAt": "2025-10-06T12:34:56.789Z",
    "libraryVersion": "3.0.0",
    "warmupPeriodCount": 19
  },
  "results": [
    { "date": "2016-01-04", "sma": null },
    { "date": "2016-01-05", "sma": null },
    { "date": "2016-01-06", "sma": null },
    { "date": "2016-02-01", "sma": 214.52331349206352 },
    { "date": "2016-02-02", "sma": 215.18015873015874 },
    { "date": "2016-02-03", "sma": 215.93571428571427 }
  ]
}
```

### Multi-property indicator (MACD)

```json
{
  "metadata": {
    "indicatorName": "Macd",
    "scenarioName": "Standard",
    "generatedAt": "2025-10-06T12:35:12.456Z",
    "libraryVersion": "3.0.0",
    "warmupPeriodCount": 33
  },
  "results": [
    { "date": "2016-01-04", "macd": null, "signal": null, "histogram": null },
    { "date": "2016-01-05", "macd": null, "signal": null, "histogram": null },
    { "date": "2016-02-15", "macd": 1.2345, "signal": 0.9876, "histogram": 0.2469 },
    { "date": "2016-02-16", "macd": 1.4567, "signal": 1.0234, "histogram": 0.4333 }
  ]
}
```

## Serialization rules

### Property naming

- Use camelCase for all JSON property names
- Metadata fields: `indicatorName`, `scenarioName`, `generatedAt`, `libraryVersion`, `warmupPeriodCount`
- Result properties: Match C# property names converted to camelCase (e.g., `Sma` → `sma`, `Macd` → `macd`)

### Property order

- **Deterministic alphabetical ordering** within each object
- Metadata object: Properties sorted alphabetically
- Result objects: `date` property first, then remaining properties alphabetically

### Numeric precision

- Full double precision preserved (no rounding)
- Scientific notation allowed for very large/small numbers
- Null values explicitly serialized (not omitted)

### Formatting

- WriteIndented = true (2-space indentation)
- UTF-8 encoding without BOM
- Unix line endings (LF) preferred
- Each result object on separate lines for diff-friendly formatting

## Validation requirements

Valid baseline files must:

1. Parse as valid JSON
2. Contain all required metadata fields
3. Have non-empty results array
4. Include explicit date for each result
5. Have consistent property names across all results
6. Use ISO 8601 format for dates and timestamps
7. Serialize null values explicitly during warmup period

## Design rationale

### Why camelCase?

- Consistency with JavaScript/TypeScript ecosystem
- Standard JSON convention for REST APIs
- Improves readability in code review diffs

### Why alphabetical property order?

- Deterministic output reduces git diff noise
- Makes visual inspection easier
- Simplifies automated validation

### Why explicit nulls?

- Complete representation of indicator behavior during warmup
- Enables validation that warmup period calculation is correct
- Prevents confusion between missing values and warmup nulls

### Why full precision?

- Captures floating-point behavior precisely
- Enables strict comparison mode (tolerance = 0)
- Documents expected precision for future reference

## Usage guidelines

### When to regenerate baselines

- After intentional algorithm changes
- When upgrading to new .NET version
- When test data changes
- During initial baseline creation

### Reviewing baseline changes

- Check metadata fields (version, warmup count)
- Verify numeric changes are expected
- Confirm property names match indicator properties
- Validate that null patterns match warmup period

### Common issues

- **Extra decimal places**: Full precision may show many digits
- **Date alignment**: Ensure dates match test data exactly
- **Property case**: Must match camelCase convention
- **Missing nulls**: Warmup nulls must be explicitly present

---
Last updated: October 6, 2025
