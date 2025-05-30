# Indicator Catalog Listing Guide

This guide explains how to implement catalog listings for indicators, including those that support multiple styles (Series, Stream, Buffer), in the Stock.Indicators catalog system.

## Overview

The catalog system uses a **one-listing-per-style approach** where indicators that provide multiple implementation styles within a single class create separate catalog listings for each supported style. Each listing has a consistent identifier across styles. This approach enables:

- Clear identification of each indicator style variant
- Accurate searching and filtering by specific styles
- Consistent catalog representation without ambiguity

## Implementation Approach

For indicators that implement multiple styles, create separate listing properties for each supported style. Each listing should use the same `IndicatorId` across all styles.

### Example: Exponential Moving Average (EMA)

The EMA indicator supports Series, Stream, and Buffer styles. Here's how to implement the catalog listings:

```csharp
public static partial class Ema
{
    [SeriesIndicator("EMA")]
    public static IReadOnlyList<EmaResult> GetEma(
      this IReadOnlyList<Quote> quotes,
      int lookbackPeriods,
      decimal? smoothingFactor = null)
    {
        // Series implementation
    }

    [StreamIndicator("EMA")]
    public static EmaCalculator GetEma(int lookbackPeriods)
    {
        // Stream implementation
        return new EmaCalculator(lookbackPeriods);
    }

    [BufferIndicator("EMA")]
    public static EmaBuffer GetEmaBuffer(int lookbackPeriods, int size)
    {
        // Buffer implementation
        return new EmaBuffer(lookbackPeriods, size);
    }

    /// <summary>
    /// Catalog listing for the Exponential Moving Average (EMA) Series indicator.
    /// </summary>
    public static readonly IndicatorListing SeriesListing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA")
        .WithStyle(Style.Series)
        .WithCategory(Category.MovingAverage)
        .AddParameter<int>("lookbackPeriods", "Lookback Period",
            description: "Number of periods for the EMA calculation",
            isRequired: true,
            defaultValue: 20,
            minimum: 2,
            maximum: 250)
        .AddParameter<decimal?>("smoothingFactor", "Smoothing Factor",
            description: "Optional custom smoothing factor")
        .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)
        .Build();

    /// <summary>
    /// Catalog listing for the Exponential Moving Average (EMA) Stream indicator.
    /// </summary>
    public static readonly IndicatorListing StreamListing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA")
        .WithStyle(Style.Stream)
        .WithCategory(Category.MovingAverage)
        .AddParameter<int>("lookbackPeriods", "Lookback Period",
            description: "Number of periods for the EMA calculation",
            isRequired: true,
            defaultValue: 20,
            minimum: 2,
            maximum: 250)
        .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)
        .Build();

    /// <summary>
    /// Catalog listing for the Exponential Moving Average (EMA) Buffer indicator.
    /// </summary>
    public static readonly IndicatorListing BufferListing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA")
        .WithStyle(Style.Buffer)
        .WithCategory(Category.MovingAverage)
        .AddParameter<int>("lookbackPeriods", "Lookback Period",
            description: "Number of periods for the EMA calculation",
            isRequired: true,
            defaultValue: 20,
            minimum: 2,
            maximum: 250)
        .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)
        .Build();
}
```

## Key Principles

### Consistent Indicator Identifiers

Each indicator should use the same identifier across all supported styles:

- Series style: `"EMA"`
- Stream style: `"EMA"`
- Buffer style: `"EMA"`

The style is already specified using the `.WithStyle()` method, so there's no need to include the style in the identifier.

### Consistent Naming Convention

Follow this pattern for multi-style indicators:

- Property names: `SeriesListing`, `StreamListing`, `BufferListing`
- Identifier format: `"{IndicatorName}"` (The style is not part of the ID)

### Style-Specific Parameters

While most parameters will be shared across styles, some parameters may be style-specific:

- Series and Stream styles typically share the same parameters
- Buffer style may include additional parameters like buffer size

## Usage Examples

### Registering style-specific indicators

```csharp
// Register each style separately
IndicatorRegistry.Register(Ema.SeriesListing);
IndicatorRegistry.Register(Ema.StreamListing);
IndicatorRegistry.Register(Ema.BufferListing);
```

### Finding indicators by style and ID

```csharp
// Find all Series style indicators
var seriesIndicators = IndicatorRegistry.GetByStyle(Style.Series);

// Find all Stream style indicators
var streamIndicators = IndicatorRegistry.GetByStyle(Style.Stream);

// Find all Buffer style indicators
var bufferIndicators = IndicatorRegistry.GetByStyle(Style.Buffer);

// Find the EMA indicator (returns all styles)
var emaIndicators = IndicatorRegistry.GetById("EMA");

// Find a specific EMA style
var emaSeries = IndicatorRegistry.GetById("EMA").FirstOrDefault(i => i.Style == Style.Series);
var emaStream = IndicatorRegistry.GetById("EMA").FirstOrDefault(i => i.Style == Style.Stream);
var emaBuffer = IndicatorRegistry.GetById("EMA").FirstOrDefault(i => i.Style == Style.Buffer);
```

## Automatic Generation

The `CatalogGenerator` can automatically detect classes with multiple indicator attribute styles and generate the appropriate style-specific listing properties. When a class has multiple attributes like `[SeriesIndicator("EMA")]`, `[StreamIndicator("EMA")]`, and `[BufferIndicator("EMA")]`, the generator will create `SeriesListing`, `StreamListing`, and `BufferListing` properties automatically, all using the same base ID ("EMA").

This automated approach ensures consistency and reduces manual effort while maintaining the one-listing-per-style principle.

## Technical Details

Multi-style support is implemented through:

1. **Style-Specific Listing Properties** - Each style (Series, Stream, Buffer) has its own dedicated listing property.
2. **Consistent Identifiers** - Each style variant uses the same base identifier (e.g., "EMA"). The style is differentiated by the `Style` property.
3. **Enhanced CatalogGenerator** - Automatically detects multi-style indicator classes and generates style-specific listing properties with consistent IDs.
4. **Registry Discovery** - Registry methods discover and register multiple listing properties per indicator class.
5. **Style-Based Filtering** - Catalog queries can efficiently filter by specific styles to find exact implementations needed.

This architecture ensures each indicator style is treated as a distinct entity in the catalog while maintaining the logical grouping within the same indicator class.
