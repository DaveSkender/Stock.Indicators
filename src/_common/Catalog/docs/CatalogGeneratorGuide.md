// filepath: d:\Repos\Stock.Indicators\src\_common\Catalog\Docs\CatalogGeneratorGuide.md
# CatalogGenerator Usage Guide

The Stock.Indicators catalog system includes a source generator (`CatalogGenerator`) that automatically generates catalog listings for indicators marked with the appropriate attributes. This guide explains how to use the generator effectively and resolve common issues.

## How to Add Attributes to Indicator Methods

To make your indicator discoverable by the catalog system, you need to add an appropriate attribute to your implementation method:

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
```

### For Stream-Style Indicators

```csharp
[StreamIndicator("STOCH")]
public static StochCalculator GetStoch()
{
    return new StochCalculator();
}
```

### For Buffer-Style Indicators

```csharp
[BufferIndicator("ADL")]
public static AdlCalculator GetAdl(int lookbackPeriods)
{
    return new AdlCalculator(lookbackPeriods);
}
```

## Choosing Between Manual Listing and Generated Listing

The CatalogGenerator automatically creates a `Listing` property for your indicator class if one doesn't already exist. However, in some cases, you might want to create a manual listing instead.

### When to Use the Generator

Use the automatic generation when:
- Your indicator follows standard implementation patterns
- The default metadata extraction is sufficient
- You want to minimize boilerplate code

### When to Create a Manual Listing

Create a manual listing when:
- You need fine-grained control over the catalog metadata
- Your indicator has complex parameters or results that need special handling
- You want to add custom categories or additional documentation
- You need to specify minimum/maximum values or default values for parameters

### Manual Listing Example

```csharp
public static partial class MyIndicator
{
    // This will be used instead of a generated listing
    public static readonly IndicatorListing Listing = new IndicatorListingBuilder()
        .WithName("My Custom Indicator")
        .WithId("MCI")
        .WithStyle(Style.Series)
        .WithCategory(Category.Custom)
        // Add detailed parameter information
        .AddParameter<int>(
            parameterName: "lookbackPeriods",
            displayName: "Lookback Period",
            description: "Number of periods for calculation",
            isRequired: true,
            minimum: 1)
        // Add detailed result information
        .AddResult(
            dataName: "Value", 
            displayName: "Value",
            dataType: ResultType.Decimal,
            isDefault: true)
        .Build();
}
```

## Resolving Common Generation Errors

### CS0111: Type Already Defines a Member

This error occurs when the generator tries to create a method that already exists in the class.

**Error:**
```
CS0111: Type already defines a member called 'CreateListing' with the same parameter types
```

**Solution:**
1. Make sure your class is marked as `partial`
2. If you have a manual `Listing` property, the generator should detect it and skip generation
3. Check if your class structure is correct and the generator can properly detect your listing

### CS0102: Type Already Contains a Definition

This error occurs when the generator tries to create a `Listing` property that already exists.

**Error:**
```
CS0102: Type already contains a definition for 'Listing'
```

**Solution:**
1. If you have a manual `Listing` property, ensure it's defined correctly
2. Check if you're using the correct namespace for your indicator class

### CS0260: Missing Partial Modifier

This error occurs when the generator tries to extend a class that isn't marked as partial.

**Error:**
```
CS0260: Missing partial modifier on declaration of type 'MyIndicator'
```

**Solution:**
Add the `partial` modifier to your indicator class:

```csharp
// Before
public static class MyIndicator
{
    // Methods
}

// After
public static partial class MyIndicator
{
    // Methods
}
```

### Multiple Attributed Methods in One Class

If you have multiple methods with indicator attributes in a single class, you might encounter errors if the generator tries to create multiple listings.

**Solution:**
1. Create a manual `Listing` property that covers all methods
2. Split the methods into separate partial classes
3. Only apply the attribute to the primary method

## Best Practices

1. **Mark Classes as Partial**: Always use the `partial` modifier for classes with attributed methods
2. **Consistent Naming**: Keep parameter names in your method signatures consistent with your listing
3. **XML Documentation**: Add XML documentation to your methods to improve the generated listings
4. **Test Your Listings**: Verify that your indicator appears in the catalog after registration
5. **Keep Listings Updated**: Update your manual listings when you change method signatures

## Troubleshooting Tips

If you're having issues with the CatalogGenerator:

1. **Check Build Output**: Look for specific error messages in the build output
2. **Use Diagnostics**: The catalog system includes analyzers that will report issues with your listings
3. **Verify Registration**: Make sure your assembly is properly registered with the catalog system
4. **Restart IDE**: Sometimes Visual Studio needs to be restarted for source generators to work properly
5. **Clean Solution**: Try cleaning your solution and rebuilding to reset the generator cache
