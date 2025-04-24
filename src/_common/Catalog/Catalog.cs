namespace Skender.Stock.Indicators;
#pragma warning disable CA1308 // suppress casing rule

/// <summary>
/// Provides methods for accessing and working with indicator listings.
/// </summary>
public static class Catalog
{
    /// <summary>
    /// Gets all indicators in the catalog, excluding test indicators.
    /// </summary>
    /// <returns>
    /// A sorted list of indicator listings.
    /// </returns>
    public static IReadOnlyList<IndicatorListing> GetIndicators() =>
        GetIndicators(includeTestIndicators: false);

    /// <summary>
    /// Gets indicators from the catalog with option to include test indicators.
    /// </summary>
    /// <param name="includeTestIndicators">Whether to include test indicators in the catalog.</param>
    /// <returns>
    /// A sorted list of indicator listings.
    /// </returns>
    internal static IReadOnlyList<IndicatorListing> GetIndicators(bool includeTestIndicators)
    {
        IEnumerable<IndicatorListing> indicators;

        // Filter out test indicators if requested
        if (!includeTestIndicators)
        {
            // test indicator UIIDs
            string[] testUiids = GeneratedCatalog.TestIndicators
                .Select(x => x.Uiid)
                .ToArray();

            indicators = GeneratedCatalog.Indicators
                .Where(x => !testUiids.Contains(x.Uiid));
        }
        else
        {
            // Include all indicators
            indicators = GeneratedCatalog.Indicators;
        }

        // Return sorted catalog
        return [.. indicators.OrderBy(x => x.Name)];
    }

    /// <summary>
    /// Gets indicators with URLs for API endpoints based on the provided base URL.
    /// </summary>
    /// <param name="baseUrl">
    /// The base URL for the indicator endpoints. Example: <c>https://example.com</c>.
    /// Explicit 'null' will generate relative URLs.
    /// </param>
    /// <returns>
    /// A sorted list of indicator listings with endpoint URLs.
    /// </returns>
    public static IReadOnlyList<IndicatorListing> GetIndicatorsWithEndpoints(Uri baseUrl)
    {
        string baseEndpoint = baseUrl?.ToString().TrimEnd('/') ?? string.Empty;
        List<IndicatorListing> listing = [];

        // Add indicators with base URL
        foreach (IndicatorListing indicator in GetIndicators())
        {
            listing.Add(new IndicatorListing(baseEndpoint) {
                Name = indicator.Name,
                Uiid = indicator.Uiid,
                UiidEndpoint = $"{baseEndpoint}/{indicator.Uiid.ToLowerInvariant()}",
                Category = indicator.Category,
                ChartType = indicator.ChartType,
                ChartConfig = indicator.ChartConfig,
                Order = indicator.Order,
                Parameters = indicator.Parameters,
                Results = indicator.Results,
                LegendTemplate = indicator.LegendTemplate
            });
        }

        return [.. listing.OrderBy(x => x.Name)];
    }

    /// <summary>
    /// Validates that each UIID is unique within the catalog.
    /// </summary>
    /// <param name="catalog">The catalog to validate.</param>
    public static void ValidateUniqueUIID(
        this IEnumerable<IndicatorListing> catalog)
    {
        List<string> duplicateUIIDs = catalog
            .GroupBy(x => x.Uiid)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateUIIDs.Count != 0)
        {
            throw new InvalidOperationException(
                $"Duplicate UIIDs found: {string.Join(", ", duplicateUIIDs)} TEST");
        }
    }

    /// <summary>
    /// Generates a default oscillator chart configuration.
    /// </summary>
    /// <param name="min">The minimum value for the Y-axis.</param>
    /// <param name="max">The maximum value for the Y-axis.</param>
    /// <param name="upperThreshold">The upper threshold value.</param>
    /// <param name="lowerThreshold">The lower threshold value.</param>
    /// <returns>A <see cref="ChartConfig"/> object with the specified parameters.</returns>
    internal static ChartConfig GetOscillatorConfig(
        float min = 0,
        float max = 100,
        float upperThreshold = 80,
        float lowerThreshold = 20) => new() {
            MinimumYAxis = min,
            MaximumYAxis = max,
            Thresholds = [
                new ChartThreshold {
                    Value = upperThreshold,
                    Color = ChartColors.ThresholdRed,
                    Style = "dash",
                    Fill = new ChartFill {
                        Target = "+2",
                        ColorAbove = "transparent",
                        ColorBelow = ChartColors.ThresholdGreen
                    }
                },
                new ChartThreshold {
                    Value = lowerThreshold,
                    Color = ChartColors.ThresholdGreen,
                    Style = "dash",
                    Fill = new ChartFill {
                        Target = "+1",
                        ColorAbove = ChartColors.ThresholdRed,
                        ColorBelow = "transparent"
                    }
                }
            ]
        };

    /// <summary>
    /// Generates a list of indicator result configurations for price bands.
    /// </summary>
    /// <param name="name">The name of the price band.</param>
    /// <param name="color">The color of the price band. If null, a default color is used.</param>
    /// <returns>A list of <see cref="IndicatorResultConfig"/> objects.</returns>
    internal static List<IndicatorResultConfig> GetPriceBandResults(string name, string? color = null)
    {
        color ??= ChartColors.StandardOrange;

        return [
            new IndicatorResultConfig {
                DisplayName = "Upper Band",
                TooltipTemplate = $"{name} Upper Band",
                DataName = "upperBand",
                DataType = "number",
                LineType = "solid",
                LineWidth = 1,
                DefaultColor = color,
                Fill = new ChartFill {
                    Target = "+2",
                    ColorAbove = ChartColors.DarkGrayTransparent,
                    ColorBelow = ChartColors.DarkGrayTransparent
                }
            },
            new IndicatorResultConfig {
                DisplayName = "Centerline",
                TooltipTemplate = $"{name} Centerline",
                DataName = "centerline",
                DataType = "number",
                LineType = "dash",
                LineWidth = 1,
                DefaultColor = color
            },
            new IndicatorResultConfig {
                DisplayName = "Lower Band",
                TooltipTemplate = $"{name} Lower Band",
                DataName = "lowerBand",
                DataType = "number",
                LineType = "solid",
                LineWidth = 1,
                DefaultColor = color
            }
        ];
    }

    /// <summary>
    /// Generates a list of indicator result configurations for moving averages.
    /// </summary>
    /// <param name="name">The name of the moving average.</param>
    /// <param name="color">The color of the moving average. If null, a default color is used.</param>
    /// <returns>A list of <see cref="IndicatorResultConfig"/> objects.</returns>
    internal static List<IndicatorResultConfig> GetMovingAverageResults(string name, string? color = null)
    {
        color ??= ChartColors.StandardBlue;

        return [
            new IndicatorResultConfig {
                DisplayName = $"{name} Moving Average",
                TooltipTemplate = $"{name}([P1])",
                DataName = "movingAverage",
                DataType = "number",
                LineType = "solid",
                LineWidth = 2,
                DefaultColor = color
            }
        ];
    }
}
