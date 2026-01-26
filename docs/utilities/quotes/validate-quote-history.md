---
title: Validate quote history
description: Advanced validation to detect duplicates and bad data in quotes.
---

# Validate quote history

`quotes.Validate()` is an advanced check of your `IReadOnlyList<IQuote> quotes`. It will check for duplicate dates and other bad data and will throw an `InvalidQuotesException` if validation fails.

## Syntax

```csharp
IReadOnlyList<Quote> validatedQuotes = quotes.Validate();
```

## Returns

**IReadOnlyList\<Quote\>** - The validated quote collection if all checks pass.

## Throws

**InvalidQuotesException** - Thrown when validation fails, with details about the validation error.

## Usage

### Standalone validation

```csharp
try
{
  IReadOnlyList<Quote> validatedQuotes = quotes.Validate();
  // proceed with valid quotes
}
catch (InvalidQuotesException ex)
{
  Console.WriteLine($"Invalid quotes: {ex.Message}");
  // handle validation failure
}
```

### Inline with chaining

```csharp
// validate and use in one expression
var results = quotes
  .Validate()
  .Use(CandlePart.HL2)
  .ToRsi(14);
```

## Validation checks

The `.Validate()` method performs several checks:

### Duplicate timestamps

Detects quotes with identical timestamps:

```csharp
// This will throw InvalidQuotesException
var badQuotes = new List<Quote> {
  new() { Timestamp = DateTime.Parse("2024-01-01"), Close = 100 },
  new() { Timestamp = DateTime.Parse("2024-01-01"), Close = 101 } // duplicate
};

badQuotes.Validate(); // throws
```

### Missing required data

Ensures all required fields are present and valid:

- Timestamp must be set
- Price values must be non-negative
- High must be >= Low
- High must be >= Open and Close
- Low must be <= Open and Close

### Chronological order

While not strictly enforced, the validator will warn about significantly out-of-order quotes that could indicate data issues.

## When to use

::: tip When validation is useful

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
var quotes = await FetchFromApi();

// ensure API data is valid
var validQuotes = quotes.Validate();

// now safe to calculate
var smaResults = validQuotes.ToSma(20);
```

### User-uploaded files

Validate user-uploaded CSV or JSON data:

```csharp
IEnumerable<Quote> ImportQuotesFromFile(string filePath)
{
  var quotes = ParseCsvFile(filePath);
  
  try
  {
    return quotes.Validate();
  }
  catch (InvalidQuotesException ex)
  {
    throw new InvalidDataException(
      $"Invalid quote file: {ex.Message}", ex);
  }
}
```

### Pipeline validation

Add validation as a quality gate in data processing pipelines:

```csharp
var processedQuotes = rawData
  .Select(TransformToQuote)
  .ToList()
  .Validate()  // ensure transformation produced valid data
  .ToSortedList();
```

## Error handling

### Catching specific issues

```csharp
try
{
  quotes.Validate();
}
catch (InvalidQuotesException ex)
{
  if (ex.Message.Contains("duplicate"))
  {
    // handle duplicate timestamps
    quotes = RemoveDuplicates(quotes);
  }
  else if (ex.Message.Contains("order"))
  {
    // handle sort order issues
    quotes = quotes.ToSortedList();
  }
  else
  {
    throw; // re-throw other validation errors
  }
}
```

## Alternatives

If `.Validate()` is too strict for your use case:

- Use [Sort quotes](/utilities/quotes/sort-quotes) to fix ordering issues
- Implement custom validation logic for your specific requirements
- Pre-process data to remove known issues before validation

## Related utilities

- [Quote utilities overview](/utilities/quotes/)
- [Sort quotes](/utilities/quotes/sort-quotes) - Fix chronological order
- [Resize quote history](/utilities/quotes/resize-quote-history) - Aggregate bars
