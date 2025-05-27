/*
 * NOTE: This file contains example code for documentation purposes only.
 * It is not part of the actual implementation and is not intended to be run.
 * Some code elements are simplified for clarity and may not follow all best practices.
 * See the actual implementation files for proper usage patterns.
 */

namespace Skender.Stock.Indicators;

/// <summary>
/// Examples of using the Stock.Indicators catalog system for documentation purposes.
/// These examples illustrate common use cases but are not part of the actual implementation.
/// </summary>
public static class CatalogExamples
{
    /// <summary>
    /// Examples of basic catalog usage patterns.
    /// </summary>
    public static void BasicCatalogUsage()
    {
        // Get all indicators in the catalog
        var allIndicators = IndicatorRegistry.GetAllIndicators();

        // Get a specific indicator by its ID
        var emaIndicator = IndicatorRegistry.GetIndicator("EMA");

        // Get indicators of a specific style
        var seriesIndicators = IndicatorRegistry.GetCatalog(Style.Series);
        var streamIndicators = IndicatorRegistry.GetCatalog(Style.Stream);
        var bufferIndicators = IndicatorRegistry.GetCatalog(Style.Buffer);

        // Get indicators by category
        var momentumIndicators = IndicatorRegistry.GetCatalog(category: Category.Momentum);
        var trendIndicators = IndicatorRegistry.GetCatalog(category: Category.Trend);

        // Search for indicators by name (case insensitive, partial matching)
        var averageIndicators = IndicatorRegistry.Search("average");

        // Advanced search with filtering
        var seriesMomentumIndicators = IndicatorRegistry.Search(
            searchTerm: "oscillator",
            style: Style.Series,
            category: Category.Momentum);
    }

    /// <summary>
    /// Examples of working with indicator listings.
    /// </summary>
    public static void WorkingWithIndicatorListings()
    {
        // Get a specific indicator
        var rsi = IndicatorRegistry.GetIndicator("RSI");

        // Accessing basic properties
        string name = rsi.Name;           // "Relative Strength Index"
        string id = rsi.Uiid;             // "RSI"
        Style style = rsi.Style;          // Style.Series
        Category category = rsi.Category; // Category.Momentum

        // Working with parameters
        foreach (var param in rsi.Parameters)
        {
            Console.WriteLine($"Parameter: {param.DisplayName} ({param.ParameterName})");
            Console.WriteLine($"  Description: {param.Description}");
            Console.WriteLine($"  Required: {param.IsRequired}");

            if (param.Minimum.HasValue)
                Console.WriteLine($"  Minimum: {param.Minimum}");

            if (param.Maximum.HasValue)
                Console.WriteLine($"  Maximum: {param.Maximum}");

            if (param.DefaultValue != null)
                Console.WriteLine($"  Default: {param.DefaultValue}");
        }

        // Getting a specific parameter
        var lookbackParam = rsi.GetParameter("lookbackPeriods");

        // Working with results
        foreach (var result in rsi.Results)
        {
            Console.WriteLine($"Result: {result.DisplayName} ({result.DataName})");
            Console.WriteLine($"  Type: {result.DataType}");
            Console.WriteLine($"  Default: {result.IsDefault}");
        }

        // Finding the default result
        var defaultResult = rsi.Results.FirstOrDefault(r => r.IsDefault);
    }

    /// <summary>
    /// Examples of using extension methods for catalog queries.
    /// </summary>
    public static void UsingExtensionMethods()
    {
        // Search by name using extension methods
        var smaIndicators = IndicatorRegistryExtensions.SearchByName("moving average");

        // Search with style filtering
        var seriesMovingAverages = IndicatorRegistryExtensions.SearchByName("moving average", Style.Series);

        // Search with category filtering
        var trendMovingAverages = IndicatorRegistryExtensions.SearchByName("moving average", Category.Trend);

        // Get indicators with a specific parameter
        var indicatorsWithPeriod = IndicatorRegistryExtensions.GetIndicatorsWithParameter("lookbackPeriods");

        // Get indicators with a parameter of specific type
        var indicatorsWithIntParam = IndicatorRegistryExtensions.GetIndicatorsWithParameterType<int>("lookbackPeriods");
    }

    /// <summary>
    /// Examples of programmatically creating indicator listings.
    /// </summary>
    public static void CreatingIndicatorListings()
    {
        // Create an indicator listing using the fluent builder API
        var customIndicator = new IndicatorListingBuilder()
            .WithName("My Custom Indicator")
            .WithId("MCI")
            .WithStyle(Style.Series)
            .WithCategory(Category.Custom)
            .AddParameter<int>(
                parameterName: "period",
                displayName: "Period",
                description: "Calculation period",
                isRequired: true,
                defaultValue: 14,
                minimum: 2)
            .AddParameter<double>(
                parameterName: "factor",
                displayName: "Factor",
                description: "Multiplier for calculation",
                isRequired: false,
                defaultValue: 2.0)
            .AddResult(
                dataName: "Value",
                displayName: "Value",
                dataType: ResultType.Decimal,
                isDefault: true)
            .Build();

        // Register a custom indicator manually
        IndicatorRegistry.Register(customIndicator);
    }
}
