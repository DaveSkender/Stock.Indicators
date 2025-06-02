# Catalog Analyzer Usage Guide

The Stock.Indicators catalog system includes a code analyzer (`CatalogAnalyzer`) that validates catalog listings against their implementations. This guide explains how to use the analyzer effectively and resolve common issues.

## How to Use Indicator Attributes

To make your indicator discoverable by the catalog system, you need to add an appropriate attribute to your implementation method and explicitly define a catalog listing:

### For Series-Style Indicators

```csharp
[SeriesIndicator("EMA")]
public static IReadOnlyList<EmaResult> ToEma<T>(
    this IReadOnlyList<T> source,
    int lookbackPeriods)
    where T : IReusable
{
    // Implementation
}

// Explicitly defined catalog listing that must align with the method signature
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    .WithName("Exponential Moving Average")
    .WithId("EMA")
    .WithStyle(Style.Series)
    .WithCategory(Category.MovingAverage)
    .AddParameter<int>("lookbackPeriods", "Lookback Period",
        description: "Number of periods for the EMA calculation",
        isRequired: true)
    .AddResult("Ema", "EMA", ResultType.Decimal, isDefault: true)
    .Build();
```

### For Stream-Style Indicators

```csharp
[StreamIndicator("STOCH")]
public static StochCalculator GetStoch()
{
    return new StochCalculator();
}

// Explicitly defined catalog listing
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    .WithName("Stochastic Oscillator")
    .WithId("STOCH")
    .WithStyle(Style.Stream)
    .WithCategory(Category.Oscillator)
    // Add parameters and results...
    .Build();
```

### For Buffer-Style Indicators

```csharp
[BufferIndicator("ADL")]
public static AdlCalculator GetAdl(int lookbackPeriods)
{
    return new AdlCalculator(lookbackPeriods);
}

// Explicitly defined catalog listing
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    .WithName("Accumulation/Distribution Line")
    .WithId("ADL")
    .WithStyle(Style.Buffer)
    .WithCategory(Category.Volume)
    .AddParameter<int>("lookbackPeriods", "Lookback Period",
        description: "Number of periods for calculation",
        isRequired: true)
    // Add results...
    .Build();
```

## Creating Explicit Catalog Listings

All indicators must have explicitly defined catalog listings. The analyzer will validate these listings against the implementation.

### Required Components

Your catalog listing must include:

1. **Basic Properties**: Name, ID, Style, Category
2. **Parameters**: All parameters from the method signature
3. **Results**: Expected result properties

### Example Implementation

```csharp
public static partial class MyIndicator
{
    [SeriesIndicator("MI")]
    public static IReadOnlyList<MyResult> ToMi(
        this IReadOnlyList<Quote> quotes,
        int lookbackPeriods)
    {
        // Implementation
    }

    // Explicit catalog listing
    public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
        .WithName("My Custom Indicator")
        .WithId("MI")
        .WithStyle(Style.Series)
        .WithCategory(Category.Custom)
        // Add parameters matching the method signature
        .AddParameter<int>(
            parameterName: "lookbackPeriods",
            displayName: "Lookback Period",
            description: "Number of periods for calculation",
            isRequired: true,
            minimum: 1)
        // Add detailed result information
        .AddResult(
            dataName: "MyValue",
            displayName: "My Value",
            dataType: ResultType.Decimal,
            isDefault: true)
        .Build();
}
```

## Resolving Common Analyzer Warnings

### SID001: Missing Indicator Listing

This diagnostic is reported when a class contains methods with indicator attributes but does not have a static `Listing` property.

**Solution:**
Create an explicit `Listing` property that matches the attributes:

```csharp
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    .WithName("My Indicator Name")
    .WithId("MI")
    .WithStyle(Style.Series)
    .WithCategory(Category.Custom)
    // Add parameters and results...
    .Build();
```

### SID002: Missing Parameters

This diagnostic is reported when a parameter exists in the implementation (method or constructor) but is missing from the indicator's `Listing` property.

**Solution:**
Add the missing parameter to your explicit `Listing` builder:

```csharp
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    // Existing code...
    .AddParameter<int>("missingParameter", "Missing Parameter",
        description: "Description of the parameter",
        isRequired: true)
    // Continue with existing code...
    .Build();
```

### SID003: Extraneous Parameters

This diagnostic is reported when a parameter exists in the indicator's `Listing` property but is missing from the implementation (method or constructor).

**Solution:**
Remove the extraneous parameter from your explicit `Listing` builder:

```csharp
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    // Include only parameters that match the method signature
    .Build();
```

### SID004: Parameter Type Mismatch

This diagnostic is reported when a parameter in the indicator's `Listing` property has a different type than the corresponding parameter in the implementation.

**Solution:**
Update the parameter type in your explicit `Listing` builder:

```csharp
// If the method has: int lookbackPeriods
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    // ...
    .AddParameter<int>("lookbackPeriods", "Lookback Period",
        description: "Number of periods for calculation",
        isRequired: true)
    // NOT .AddParameter<double> which would cause a mismatch
    // ...
    .Build();
```

### SID005: Missing Results

This diagnostic is reported when expected result properties from the return type are not defined in the indicator's `Listing` property.

**Solution:**
Add the missing results to your explicit `Listing` builder:

```csharp
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    // Existing code...
    .AddResult("MissingResultProperty", "Missing Result", ResultType.Decimal, isDefault: false)
    // Continue with existing code...
    .Build();
```

## Multi-Style Indicators

For indicators that implement multiple styles (Series, Stream, Buffer), you need to create separate listing properties for each supported style:

```csharp
public static partial class Ema
{
    [SeriesIndicator("EMA")]
    public static IReadOnlyList<EmaResult> GetEma(
        this IReadOnlyList<Quote> quotes,
        int lookbackPeriods)
    {
        // Series implementation
    }

    [StreamIndicator("EMA")]
    public static EmaCalculator GetEma(int lookbackPeriods)
    {
        // Stream implementation
    }

    // Series listing
    public static readonly IndicatorListing SeriesListing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA")
        .WithStyle(Style.Series)
        // Add parameters and results...
        .Build();

    // Stream listing
    public static readonly IndicatorListing StreamListing = new IndicatorListingBuilder()
        .WithName("Exponential Moving Average")
        .WithId("EMA")
        .WithStyle(Style.Stream)
        // Add parameters and results...
        .Build();
}
```

## Best Practices

1. **Keep Listings in Sync**: Always update your `Listing` property when changing method signatures
2. **Use Partial Classes**: Define your catalog listings in a separate file (e.g., `MyIndicator.Catalog.cs`)
3. **Parameter Names**: Ensure parameter names in the catalog match the method exactly
4. **Documentation**: Add descriptive display names and descriptions to make the catalog more useful
5. **Testing**: Write unit tests to verify your catalog listings match the implementation

## Troubleshooting Tips

If you're having issues with the CatalogAnalyzer:

1. **Check Build Output**: Look for specific error messages in the build output
2. **Use Visual Studio Error List**: View analyzer diagnostics in the Error List window
3. **Verify Listings**: Ensure your explicit listings match the implementation
4. **Restart IDE**: Sometimes Visual Studio needs to be restarted for analyzers to work properly
5. **Clean Solution**: Try cleaning your solution and rebuilding to reset the analyzer cache
