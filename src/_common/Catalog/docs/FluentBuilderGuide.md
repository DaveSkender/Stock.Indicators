# Fluent Builder API Guide

The Stock.Indicators catalog system provides a fluent builder API for creating and configuring indicator listings. This guide explains how to use the `IndicatorListingBuilder` to create well-documented indicator catalog entries.

## Basic Usage

The fluent builder pattern allows you to create an `IndicatorListing` instance using a chain of method calls:

```csharp
public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
    .WithName("Simple Moving Average")
    .WithId("SMA")
    .WithStyle(Style.Series)
    .WithCategory(Category.MovingAverage)
    .AddParameter<int>("lookbackPeriods", "Lookback Period",
        description: "Number of periods for calculation",
        isRequired: true)
    .AddResult("Sma", "SMA", ResultType.Decimal, isDefault: true)
    .Build();
```

## Core Properties

### WithName(string name)

Sets the display name of the indicator. This should be the full, human-readable name.

```csharp
.WithName("Exponential Moving Average")
```

### WithId(string id)

Sets the unique identifier for the indicator. This should be a short code, typically an acronym.

```csharp
.WithId("EMA")
```

### WithStyle(Style style)

Sets the indicator's style (Series, Stream, or Buffer). This should match the attribute used on the implementation method.

```csharp
.WithStyle(Style.Series)
```

### WithCategory(Category category)

Sets the indicator's category for classification purposes.

```csharp
.WithCategory(Category.MovingAverage)
```

## Adding Parameters

### AddParameter<T>(string parameterName, string displayName, ...)

Adds a parameter to the indicator listing. The parameters include:

- `parameterName`: Technical name of the parameter (must match method parameter name)
- `displayName`: Human-readable name for display purposes
- `description`: Detailed description of the parameter
- `isRequired`: Whether the parameter is required or optional
- `defaultValue`: Default value for optional parameters
- `minimum`: Minimum allowed value (for numeric parameters)
- `maximum`: Maximum allowed value (for numeric parameters)

```csharp
.AddParameter<int>("lookbackPeriods", "Lookback Period",
    description: "Number of periods for SMA calculation",
    isRequired: true,
    defaultValue: 20,
    minimum: 1,
    maximum: 500)
    
.AddParameter<decimal?>("threshold", "Threshold Value",
    description: "Optional threshold for signal generation",
    isRequired: false,
    defaultValue: 0.5m)
```

## Adding Results

### AddResult(string dataName, string displayName, ResultType dataType, bool isDefault)

Adds a result to the indicator listing:

- `dataName`: Technical name of the result property
- `displayName`: Human-readable name for display purposes
- `dataType`: Type of the result data (e.g., `ResultType.Decimal`, `ResultType.Boolean`)
- `isDefault`: Whether this is the primary/default result

```csharp
.AddResult("Sma", "SMA", ResultType.Decimal, isDefault: true)
.AddResult("SlowEma", "Slow EMA", ResultType.Decimal, isDefault: false)
.AddResult("FastEma", "Fast EMA", ResultType.Decimal, isDefault: false)
```

## Common Result Patterns

The builder supports extension methods for common result patterns:

```csharp
// Add standard price High, Low, Close results
.AddPriceHlcResult()

// Add signal line result
.AddSignalLineResult()

// Add upper/lower band results
.AddBandsResult()
```

## Building the Listing

### Build()

Finalizes the builder and returns the constructed `IndicatorListing`.

```csharp
.Build()
```

## Complete Example

```csharp
public static partial class Rsi
{
    public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
        .WithName("Relative Strength Index")
        .WithId("RSI")
        .WithStyle(Style.Series)
        .WithCategory(Category.Momentum)
        .AddParameter<int>("lookbackPeriods", "Lookback Period",
            description: "Number of periods for RSI calculation",
            isRequired: true,
            defaultValue: 14,
            minimum: 1)
        .AddResult("Rsi", "RSI", ResultType.Decimal, isDefault: true)
        .Build();
}
```

## Best Practices

1. **Match Method Parameters**: Ensure parameter names in the builder match the method parameter names exactly.
2. **Include Descriptive Information**: Provide meaningful display names and descriptions.
3. **Set Proper Constraints**: Include minimum/maximum values where appropriate.
4. **Separate Catalog Files**: Define your catalog listings in a separate file (e.g., `MyIndicator.Catalog.cs`).
5. **Unit Test Verification**: Write tests to verify catalog correctness.
6. **Use Partial Classes**: Keep implementation and catalog code separated but connected.

## Validation

The builder performs validation when `Build()` is called and will throw exceptions if:

- Required core properties are missing (name, ID, style)
- Parameter names don't match implementation
- Multiple default results are defined
- Invalid values are provided for constraints
