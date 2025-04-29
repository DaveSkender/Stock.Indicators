# Generators and analyzers

This is a .NET source generator that automatically creates catalog entries for indicators in the Stock.Indicators library.

## Purpose

The Generator analyzes indicator classes decorated with attribute flavors:

| Attribute Flavor | Description |
|------------------|-------------|
| `SeriesAttribute` | For indicators that operate on series data |
| `StreamAttribute` | For indicators that stream data |
| `BufferAttribute` | For indicators that use buffer-style data |
| `IndicatorAttribute` | Base attribute for all indicators |

It then generates a catalog of all indicators with their metadata, parameters, and default configurations.

## How it works

During compilation, the source generator:

1. Scans the codebase for methods and constructors with the appropriate indicator attributes
2. Extracts metadata like ID, name, and parameters
3. Generates a static class `GeneratedCatalog` with a method to access all indicators

## Analyzers

The package includes Roslyn analyzers that help enforce conventions:

| Rule ID | Description |
|---------|-------------|
| IND001  | Identifies series-style indicator methods missing the required `Series` attribute |
| IND002  | Identifies stream-style indicator methods missing the required `Stream` attribute |
| IND003  | Identifies buffer-style indicator methods missing the required `Buffer` attribute |

The analyzer intelligently identifies indicator types based on their return types:

- **Series indicators**: Methods returning collections where the element type name ends with "Result"
- **Stream indicators**: Methods returning types that implement `IStreamHub` interface
- **Buffer indicators**: Methods returning types that implement `IBufferQuote` interface

To exclude specific methods from analyzer checks, apply the `[ExcludeFromCatalog]` attribute.

## Usage

The generated catalog is accessible via:

```csharp
using Skender.Stock.Indicators;

// Get all indicators from the generated catalog
var indicators = GeneratedCatalog.GetIndicators();

// Use the indicators
foreach (var indicator in indicators)
{
    Console.WriteLine($"Found indicator: {indicator.Name} ({indicator.Uiid})");
    
    // Access parameters
    foreach (var param in indicator.Parameters)
    {
        Console.WriteLine($"  Parameter: {param.DisplayName} (default: {param.Default})");
    }
}
```

## Integration

The catalog generator is integrated as an analyzer in the main Stock.Indicators project and automatically runs during compilation.

## Benefits

- Automatic catalog generation based on code attributes
- No need for manual catalog maintenance
- Complete coverage of all indicators in the codebase
- Consistent metadata for all indicators
- Style-specific analyzer warnings for proper attribute usage
