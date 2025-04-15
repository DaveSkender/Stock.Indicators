# Stock.Indicators CatalogGenerator

This is a .NET source generator that automatically creates catalog entries for indicators in the Stock.Indicators library.

## Purpose

The CatalogGenerator analyzes indicator classes decorated with attribute flavors:
- `SeriesAttribute` (for indicator methods)
- `StreamHubAttribute` (for streaming indicators)
- `BufferAttribute` (for buffer-style indicators)

It then generates a catalog of all indicators with their metadata, parameters, and default configurations.

## How it Works

During compilation, the source generator:
1. Scans the codebase for methods and constructors with the appropriate indicator attributes
2. Extracts metadata like ID, name, and parameters
3. Generates a static class `GeneratedIndicatorCatalog` with a method to access all indicators

## Usage

The generated catalog is accessible via:

```csharp
using Skender.Stock.Indicators;

// Get all indicators from the generated catalog
var indicators = GeneratedIndicatorCatalog.GetIndicators();

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
