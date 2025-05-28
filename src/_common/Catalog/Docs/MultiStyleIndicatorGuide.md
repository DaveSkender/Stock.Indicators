# Multi-Style Indicator Support

This guide explains how to implement indicators that support multiple styles (Series, Stream, Buffer) in the Stock.Indicators catalog system.

## Overview

The catalog system now supports indicators that provide multiple implementation styles within a single class. This enables:

- Registering a single indicator with multiple style variants
- Searching and filtering that properly recognizes all supported styles
- Consistent catalog representation that avoids duplicates

## Implementation Options

### Option 1: Manual Composite Listing

For indicators that implement multiple styles, create a manual `Listing` property using the `CompositeIndicatorListingBuilder`:

```csharp
public static partial class MyMultiStyleIndicator
{
    [SeriesIndicator("MSI")]
    public static IReadOnlyList<MyResult> ToMyIndicator(this IReadOnlyList<Quote> quotes, int lookbackPeriods)
    {
        // Series implementation
    }

    [StreamIndicator("MSI")]
    public static MyCalculator GetMyIndicator(int lookbackPeriods)
    {
        // Stream implementation
        return new MyCalculator(lookbackPeriods);
    }

    public static readonly IndicatorListing Listing = new CompositeIndicatorListingBuilder()
        .WithName("My Multi-Style Indicator")
        .WithId("MSI")
        .WithStyle(Style.Series) // Primary style
        .WithSupportedStyles(Style.Series, Style.Stream) // All supported styles
        .WithCategory(Category.Custom)
        .AddParameter<int>("lookbackPeriods", "Lookback Period", 
            description: "Number of periods for calculation",
            isRequired: true)
        .AddResult("Value", "Value", ResultType.Default, isDefault: true)
        .Build();
}
```

### Option 2: Automatic Composite Listing Generation

The `CatalogGenerator` automatically detects classes with multiple indicator attribute styles and generates a composite listing:

```csharp
public static partial class MyMultiStyleIndicator
{
    [SeriesIndicator("MSI")]
    public static IReadOnlyList<MyResult> ToMyIndicator(this IReadOnlyList<Quote> quotes, int lookbackPeriods)
    {
        // Series implementation
    }

    [StreamIndicator("MSI")]
    public static MyCalculator GetMyIndicator(int lookbackPeriods)
    {
        // Stream implementation
        return new MyCalculator(lookbackPeriods);
    }
    
    // No need for manual Listing - one will be generated automatically
}
```

## Usage Examples

### Registering a composite indicator manually:

```csharp
// Create composite listing
var listing = new CompositeIndicatorListingBuilder()
    .WithName("My Indicator")
    .WithId("MI")
    .WithStyle(Style.Series)
    .WithSupportedStyles(Style.Series, Style.Stream)
    .WithCategory(Category.MovingAverage)
    .Build();

// Register in catalog
IndicatorRegistry.Register(listing);
```

### Finding indicators by style:

```csharp
// Find all indicators supporting Series style
var seriesIndicators = IndicatorRegistry.GetByStyle(Style.Series);

// Find all indicators supporting Stream style
var streamIndicators = IndicatorRegistry.GetByStyle(Style.Stream);

// Find all indicators supporting Buffer style
var bufferIndicators = IndicatorRegistry.GetByStyle(Style.Buffer);
```

## Technical Details

Multi-style support is implemented through:

1. `CompositeIndicatorListing` - extends `IndicatorListing` with support for multiple styles
2. `CompositeIndicatorListingBuilder` - specialized builder for creating composite listings
3. Enhanced CatalogGenerator - automatically detects multi-style indicator classes
4. Updated registry methods - correctly filter by style across both standard and composite listings
