# Generators and Analyzers

This package contains .NET source generators and Roslyn analyzers for the Stock.Indicators library, providing automatic catalog generation and validation rules for indicator implementations.

## Purpose

The system analyzes indicator classes decorated with attributes and provides:

- **Catalog System**: Validates indicator listings and metadata consistency
- **Validation Rules**: Ensures data integrity across indicator definitions
- **Source Generation**: Automatically creates catalog entries (planned feature)

## Supported Attributes

| Attribute | Description |
|-----------|-------------|
| `SeriesIndicatorAttribute` | For indicators that operate on series data |
| `StreamIndicatorAttribute` | For indicators that stream data |
| `BufferIndicatorAttribute` | For indicators that use buffer-style data |
| `IndicatorAttribute` | Base attribute for all indicators |

## Analyzer Rules

The package includes Roslyn analyzers that enforce catalog system conventions:

### Catalog System Rules (SID)

| Rule ID | Description | Severity |
|---------|-------------|----------|
| SID001  | Missing indicator listing property | Info |
| SID002  | Missing parameters in listing | Info |
| SID003  | Extraneous parameters in listing | Info |
| SID004  | Parameter type mismatch | Info |
| SID005  | Missing results in listing | Info |

## Usage

### Catalog System

Indicators should define a static `Listing` property:

```csharp
[SeriesIndicator("EMA")]
public static class Ema 
{
    public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA")
        .WithStyle(Style.Series)
        .AddParameter<int>("lookbackPeriods", "Lookback Period")
        .AddResult("Ema", "EMA", ResultType.Decimal, isDefault: true)
        .Build();
        
    // Implementation methods...
}
```

### Excluding Methods

To exclude specific methods from analyzer checks, apply the `[ExcludeFromCatalog]` attribute.

## Integration

The analyzers are integrated into the main Stock.Indicators project and run automatically during compilation, providing real-time validation feedback.
