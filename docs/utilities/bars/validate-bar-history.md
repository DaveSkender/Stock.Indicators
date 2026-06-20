---
title: Validate bar history
description: Advanced validation to detect duplicates and bad data in bars.
---

# Validate bar history

`bars.Validate()` is an advanced check of your `IReadOnlyList<IBar> bars`. It checks for duplicate timestamps and out-of-sequence (non-ascending) dates, and throws an `InvalidBarsException` if either is found.

## Syntax

```csharp
IReadOnlyList<Bar> validatedBars = bars.Validate();
```

## Returns

**IReadOnlyList\<Bar\>** - The validated bar collection if all checks pass.

## Throws

**InvalidBarsException** - Thrown when validation fails, with details about the validation error.

## Usage

### Standalone validation

```csharp
try
{
  IReadOnlyList<Bar> validatedBars = bars.Validate();
  // proceed with valid bars
}
catch (InvalidBarsException ex)
{
  Console.WriteLine($"Invalid bars: {ex.Message}");
  // handle validation failure
}
```

### Inline with chaining

```csharp
// validate and use in one expression
var results = bars
  .Validate()
  .Use(CandlePart.HL2)
  .ToRsi(14);
```

## Validation checks

The `.Validate()` method performs several checks:

### Duplicate timestamps

Detects bars with identical timestamps:

```csharp
// This will throw InvalidBarsException
var badBars = new List<Bar> {
  new() { Timestamp = DateTime.Parse("2024-01-01"), Close = 100 },
  new() { Timestamp = DateTime.Parse("2024-01-01"), Close = 101 } // duplicate
};

badBars.Validate(); // throws
```

### Chronological order

The validator enforces chronological order and throws `InvalidBarsException` for any out-of-order (descending) bars.

::: info When to use
Validation is useful when:

- **Importing from untrusted sources** - External APIs or user-uploaded files
- **Data quality assurance** - Production systems requiring verified data
- **Debugging data issues** - Troubleshooting unexpected indicator results
- **Before expensive calculations** - Validate once before running multiple indicators
:::

::: info Performance cost
`.Validate()` performs thorough checks and has a performance cost. Use it when data quality is uncertain, but avoid repeated validation of the same dataset.
:::

## Common use cases

### API data import

Validate data from external APIs before processing:

```csharp
var bars = await FetchFromApi();

// ensure API data is valid
var validBars = bars.Validate();

// now safe to calculate
var smaResults = validBars.ToSma(20);
```

### User-uploaded files

Validate user-uploaded CSV or JSON data:

```csharp
IEnumerable<Bar> ImportBarsFromFile(string filePath)
{
  var bars = ParseCsvFile(filePath);
  
  try
  {
    return bars.Validate();
  }
  catch (InvalidBarsException ex)
  {
    throw new InvalidDataException(
      $"Invalid bar file: {ex.Message}", ex);
  }
}
```

### Pipeline validation

Add validation as a quality gate in data processing pipelines:

```csharp
var processedBars = rawData
  .Select(TransformToBar)
  .ToList()
  .Validate()  // ensure transformation produced valid data
  .ToSortedList();
```

## Error handling

### Catching specific issues

```csharp
try
{
  bars.Validate();
}
catch (InvalidBarsException ex)
{
  if (ex.Message.Contains("Duplicate"))
  {
    // handle duplicate timestamps
    bars = RemoveDuplicates(bars);
  }
  else if (ex.Message.Contains("out of sequence"))
  {
    // handle sort order issues
    bars = bars.ToSortedList();
  }
  else
  {
    throw; // re-throw other validation errors
  }
}
```

## Alternatives

If `.Validate()` is too strict for your use case:

- Use [Sort bars](/utilities/bars/sort-bars) to fix ordering issues
- Implement custom validation logic for your specific requirements
- Pre-process data to remove known issues before validation

## Related utilities

- [Bar utilities overview](/utilities/bars/)
- [Sort bars](/utilities/bars/sort-bars) - Fix chronological order
- [Resize bar history](/utilities/bars/resize-bar-history) - Aggregate bars
