// filepath: d:\Repos\Stock.Indicators\src\_common\Catalog\Docs\AnalyzerRules.md
# Catalog System Analyzer Rules

This document describes the analyzer rules used in the Stock.Indicators catalog system and how to fix reported issues.

## SID001: Missing Indicator Listing

### Description
This diagnostic is reported when a class contains methods with indicator attributes (`SeriesIndicatorAttribute`, `StreamIndicatorAttribute`, or `BufferIndicatorAttribute`) but does not have a static `Listing` property.

### How to Fix
1. **Option 1**: Create a manual `Listing` property in a partial class:
```csharp
public static partial class MyIndicator
{
    public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
        .WithName("My Indicator Name")
        .WithId("MI")
        .WithStyle(Style.Series)
        .WithCategory(Category.Custom)
        .AddParameter<int>("lookbackPeriods", "Lookback Period", 
            description: "Number of periods for calculation",
            isRequired: true)
        // Add any other parameters from your method
        .AddResult("Value", "Value", ResultType.Default, isDefault: true)
        .Build();
}
```

2. **Option 2**: Rely on the CatalogGenerator to create the listing automatically. Ensure:
   - Your class is marked as `partial`
   - Your method has the correct indicator attribute
   - The method parameters are properly named and documented

## SID002: Missing Parameters

### Description
This diagnostic is reported when a parameter exists in the implementation (method or constructor) but is missing from the indicator's `Listing` property.

### How to Fix
Add the missing parameter to the `Listing` builder:

```csharp
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    // Existing code...
    .AddParameter<int>("missingParameter", "Missing Parameter", 
        description: "Description of the parameter",
        isRequired: true) // Set to false if the parameter has a default value
    // Continue with existing code...
    .Build();
```

Ensure the parameter name matches exactly what's in the method signature (case-sensitive).

## SID003: Extraneous Parameters

### Description
This diagnostic is reported when a parameter exists in the indicator's `Listing` property but is missing from the implementation (method or constructor).

### How to Fix
Either:
1. Remove the extraneous parameter from the `Listing` builder
2. Add the parameter to the method signature if it's actually needed

```csharp
// To remove from Listing:
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    // Keep only parameters that match the method signature
    .Build();

// OR to add to method:
[SeriesIndicator("XYZ")]
public static IReadOnlyList<ResultType> ToXyz(
    this IReadOnlyList<T> source,
    int existingParameter,
    int newlyAddedParameter) // Add the missing parameter
```

## SID004: Parameter Type Mismatch

### Description
This diagnostic is reported when a parameter in the indicator's `Listing` property has a different type than the corresponding parameter in the implementation.

### How to Fix
Update the parameter type in the `Listing` builder to match the implementation:

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

## SID005: Missing Results

### Description
This diagnostic is reported when expected result properties from the return type are not defined in the indicator's `Listing` property.

### How to Fix
Add the missing results to the `Listing` builder:

```csharp
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    // Existing code...
    .AddResult("MissingResultProperty", "Missing Result", ResultType.Decimal, isDefault: false)
    // Continue with existing code...
    .Build();
```

For a result that should be the default (primary) result, set `isDefault: true`.

## General Best Practices

1. **Keep Listings in Sync**: Always update your `Listing` property when changing method signatures.
2. **Use Partial Classes**: Define your catalog listings in a separate file (e.g., `MyIndicator.Catalog.cs`).
3. **Parameter Names**: Ensure parameter names in the catalog match the method exactly.
4. **Documentation**: Add descriptive display names and descriptions to make the catalog more useful.
5. **Testing**: Write unit tests to verify your catalog listings match the implementation.
