# Multi-Style Indicator Support

This guide explains how to implement indicators that support multiple styles (Series, Stream, Buffer) in the Stock.Indicators catalog system.

## Overview

The catalog system supports indicators that provide multiple implementation styles within a single class. For each style implemented, a separate catalog listing should be created with a unique identifier. This approach enables:

- Clear identification of each indicator style variant
- Accurate searching and filtering by specific styles
- Consistent catalog representation without ambiguity

## Implementation Approach

For indicators that implement multiple styles, create separate listing properties for each supported style. Each listing should have a unique identifier that includes the style suffix.

### Example: Exponential Moving Average (EMA)

The EMA indicator supports Series, Stream, and Buffer styles. Here's how to implement the catalog listings:

```csharp
public static partial class Ema
{
    [SeriesIndicator("EMA")]
    public static IReadOnlyList<EmaResult> GetEma(this IReadOnlyList<Quote> quotes, int lookbackPeriods, decimal? smoothingFactor = null)
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
        .WithId("EMA-Series")
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
        .WithId("EMA-Stream")
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
        .WithId("EMA-Buffer")
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

### Unique Identifiers per Style

Each style should have a unique identifier that clearly indicates the implementation type:
- Series style: `"EMA-Series"`
- Stream style: `"EMA-Stream"` 
- Buffer style: `"EMA-Buffer"`

### Consistent Naming Convention

Follow this pattern for multi-style indicators:
- Property names: `SeriesListing`, `StreamListing`, `BufferListing`
- Identifier format: `"{IndicatorName}-{Style}"`

### Style-Specific Parameters

While most parameters will be shared across styles, some parameters may be style-specific:
- Series and Stream styles typically share the same parameters
- Buffer style may include additional parameters like buffer size

## Usage Examples

### Registering style-specific indicators:

```csharp
// Register each style separately
IndicatorRegistry.Register(Ema.SeriesListing);
IndicatorRegistry.Register(Ema.StreamListing);
IndicatorRegistry.Register(Ema.BufferListing);
```

### Finding indicators by style:

```csharp
// Find all Series style indicators
var seriesIndicators = IndicatorRegistry.GetByStyle(Style.Series);

// Find all Stream style indicators  
var streamIndicators = IndicatorRegistry.GetByStyle(Style.Stream);

// Find all Buffer style indicators
var bufferIndicators = IndicatorRegistry.GetByStyle(Style.Buffer);

// Find specific EMA implementations
var emaSeries = IndicatorRegistry.GetById("EMA-Series");
var emaStream = IndicatorRegistry.GetById("EMA-Stream");
var emaBuffer = IndicatorRegistry.GetById("EMA-Buffer");
```

## Automatic Generation

The `CatalogGenerator` can automatically detect classes with multiple indicator attribute styles and generate the appropriate style-specific listing properties. When a class has multiple attributes like `[SeriesIndicator("EMA")]`, `[StreamIndicator("EMA")]`, and `[BufferIndicator("EMA")]`, the generator will create `SeriesListing`, `StreamListing`, and `BufferListing` properties automatically.

This automated approach ensures consistency and reduces manual effort while maintaining the one-listing-per-style principle.
```

## Technical Details

Multi-style support is implemented through:

1. **Style-Specific Listing Properties** - Each style (Series, Stream, Buffer) has its own dedicated listing property
2. **Unique Identifiers** - Each style variant gets a unique identifier following the pattern `{IndicatorName}-{Style}`
3. **Enhanced CatalogGenerator** - Automatically detects multi-style indicator classes and generates style-specific listing properties
4. **Registry Discovery** - Registry methods discover and register multiple listing properties per indicator class
5. **Style-Based Filtering** - Catalog queries can efficiently filter by specific styles to find exact implementations needed

This architecture ensures each indicator style is treated as a distinct entity in the catalog while maintaining the logical grouping within the same indicator class.
