# Baseline JSON file format specification

**Feature**: Static series regression baselines  
**Version**: 1.0  
**Date**: October 6, 2025

## Overview

This document defines the JSON format for regression baseline files. These files store canonical indicator outputs for comparison against current test results to detect unintended behavioral drift.

## File naming and location

- **Pattern**: `{IndicatorName}.Baseline.json`
- **Examples**: `Sma.Baseline.json`, `Macd.Baseline.json`
- **Location**: Colocated with indicator tests (e.g., `tests/indicators/s-z/Sma/Sma.Baseline.json`)
- **Case sensitivity**: Matches indicator class name (e.g., `Sma`, not `SMA`)

## JSON schema

Baseline files contain a JSON array of indicator result objects that can be directly deserialized to the indicator's result type (e.g., `List<SmaResult>`):

```json
[
  {
    "timestamp": "2016-01-04T00:00:00",
    "propertyName1": null,
    "propertyName2": null
  },
  {
    "timestamp": "2016-01-05T00:00:00",
    "propertyName1": 214.52,
    "propertyName2": 1.23
  }
]
```

### Result object structure

Each object in the array represents one date's calculation and maps directly to the indicator's result class:

- **timestamp** (required): Date/time in ISO 8601 format
  - Maps to `Timestamp` property in C# result classes (e.g., `SmaResult.Timestamp`)
  - JSON uses camelCase via System.Text.Json's PropertyNamingPolicy.CamelCase
  - Older indicators may use `Date` property name - both will serialize as "timestamp" or "date" in JSON
- **indicator properties** (variable): All properties from the indicator's result class
  - Property names match C# property names (PascalCase in C#, camelCase in JSON)
  - Values are either `number` (double precision) or `null` (during warmup)
  - Null values MUST be explicitly serialized (not omitted)

## Example baseline files

### Single-property indicator (SMA)

```json
[
  {
    "timestamp": "2016-01-04T00:00:00",
    "sma": null
  },
  {
    "timestamp": "2016-01-05T00:00:00",
    "sma": null
  },
  {
    "timestamp": "2016-02-01T00:00:00",
    "sma": 214.52331349206352
  },
  {
    "timestamp": "2016-02-02T00:00:00",
    "sma": 215.18015873015874
  }
]
```

### Multi-property indicator (MACD)

```json
[
  {
    "timestamp": "2016-01-04T00:00:00",
    "macd": null,
    "signal": null,
    "histogram": null,
    "fastEma": null,
    "slowEma": null
  },
  {
    "timestamp": "2016-02-15T00:00:00",
    "macd": 1.2345,
    "signal": 0.9876,
    "histogram": 0.2469,
    "fastEma": 216.34,
    "slowEma": 215.10
  }
]
```

## Serialization rules

### Direct deserialization

Baseline files are designed to deserialize directly to `List<TResult>` where `TResult` is the indicator's result type:

```csharp
// Example: Deserializing SMA baseline
List<SmaResult> baseline = JsonSerializer.Deserialize<List<SmaResult>>(json);
```

### Framework requirements

- .NET 8.0+ (current multi-target: net9.0, net8.0)
- System.Text.Json (included in framework, no separate package needed)
- PropertyNamingPolicy.CamelCase supported in all target frameworks

### Property naming

- JSON uses camelCase convention (System.Text.Json default with `PropertyNamingPolicy.CamelCase`)
- C# result classes use PascalCase
- Automatic conversion during serialization/deserialization

### Numeric precision

- Full double precision preserved (no rounding)
- Scientific notation allowed for very large/small numbers
- Null values explicitly serialized (not omitted)

### Formatting

- WriteIndented = true (human-readable with proper indentation)
- UTF-8 encoding
- ISO 8601 format for timestamps (System.Text.Json default: "yyyy-MM-ddTHH:mm:ss" for DateTime without timezone, or with timezone offset for DateTimeOffset)
- DateTime.Kind should be specified (recommend Utc or Unspecified for consistency)

## Validation requirements

Valid baseline files must:

1. Parse as valid JSON array
2. Contain at least one result object
3. Each object must have a `timestamp` property
4. Property names must match the indicator's result class properties (camelCase in JSON)
5. Use ISO 8601 format for timestamps
6. Serialize null values explicitly during warmup period

## Design rationale

### Why direct result class mapping?

- **Simplicity**: No custom wrapper types needed
- **Consistency**: Uses existing indicator result models
- **Type safety**: Leverages C# type system for validation
- **Maintainability**: Changes to result models automatically reflected in baselines

### Why colocated with tests?

- **Discoverability**: Easy to find baseline alongside test code
- **Organization**: Follows existing test file patterns
- **Clarity**: Clear which baseline belongs to which indicator

### Why explicit nulls?

- **Complete representation**: Shows indicator behavior during warmup
- **Validation**: Enables verification of warmup period calculations
- **Consistency**: Prevents confusion between missing values and warmup nulls

### Why full precision?

- **Accuracy**: Captures floating-point behavior precisely
- **Mathematical precision**: NON-NEGOTIABLE per Constitution Principle I—exact binary equality required
- **Documentation**: Records expected precision for future reference

## Usage guidelines

### When to regenerate baselines

- After intentional algorithm changes
- When upgrading to new .NET version
- When test data changes
- During initial baseline creation for new indicators

### Reviewing baseline changes

- Verify numeric changes are expected and documented
- Confirm property names match indicator result properties
- Validate that null patterns match warmup period
- Check timestamp alignment with test data

### Common issues

- **Property name mismatches**: Ensure camelCase matches PascalCase C# properties
- **Missing nulls**: Warmup nulls must be explicitly present
- **Timestamp format**: Must use ISO 8601 format
- **Type mismatches**: All numeric values must be valid doubles or null
- **Timezone inconsistency**: Ensure test data uses consistent DateTime.Kind to avoid serialization format variations

---
Last updated: October 6, 2025
