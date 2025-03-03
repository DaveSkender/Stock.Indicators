namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating indicator listings.
/// <remarks>
/// It contains opinionated parameter defaults, ranges, colors, and other values.
/// </remarks>
/// </summary>
public static class Metacatalog
{
    /// <inheritdoc cref="IndicatorCatalog(Uri)"/>"
    public static IReadOnlyList<IndicatorListing> IndicatorCatalog()
    {
        // Get the base catalog
        var catalog = new List<IndicatorListing>();

        // Add hardcoded indicators
        catalog.AddRange([
            // Exponential Moving Average (EMA)
            new IndicatorListing {
                Name = "Exponential Moving Average (EMA)",
                Uiid = "EMA",
                Category = "moving-average",
                ChartType = "overlay",
                Parameters = [
                    new IndicatorParamConfig
                    {
                        DisplayName = "Lookback Periods",
                        ParamName = "lookbackPeriods",
                        DataType = "int",
                        DefaultValue = 20,
                        Minimum = 2,
                        Maximum = 250
                    }
                ],
                Results = [
                    new IndicatorResultConfig
                    {
                        DisplayName = "Exponential Moving Average",
                        TooltipTemplate = "EMA([P1])",
                        DataName = "ema",
                        DataType = "number",
                        LineType = "solid",
                        DefaultColor = ChartColors.StandardBlue
                    }
                ]
            },

            // Simple Moving Average (SMA)
            new IndicatorListing {
                Name = "Simple Moving Average (SMA)",
                Uiid = "SMA",
                Category = "moving-average",
                ChartType = "overlay",
                Parameters = [
                    new IndicatorParamConfig
                    {
                        DisplayName = "Lookback Periods",
                        ParamName = "lookbackPeriods",
                        DataType = "int",
                        DefaultValue = 20,
                        Minimum = 2,
                        Maximum = 250
                    }
                ],
                Results = [
                    new IndicatorResultConfig
                    {
                        DisplayName = "Simple Moving Average",
                        TooltipTemplate = "SMA([P1])",
                        DataName = "sma",
                        DataType = "number",
                        LineType = "solid",
                        DefaultColor = ChartColors.StandardBlue
                    }
                ]
            }
        ]);

        // Add generated indicators
        var generatedCatalog = GeneratedIndicatorCatalog.GetIndicators();
        catalog.AddRange(generatedCatalog);

        // Return sorted catalog
        return [.. catalog.OrderBy(x => x.Name)];
    }

    /// <summary>
    /// Generates a list of indicator with optional base URL.
    /// </summary>
    /// <param name="baseUrl">
    /// The base URL for the indicator endpoints. Example: <c>https://example.com</c>.
    /// Explicit 'null' will generate relative URLs.
    /// </param>
    /// <returns>
    /// An enumerable collection of <see cref="IndicatorListing"/> objects.
    /// </returns>
    public static IReadOnlyList<IndicatorListing> IndicatorCatalog(Uri baseUrl)
    {
        string baseEndpoint = baseUrl?.ToString().TrimEnd('/') ?? string.Empty;
        var listing = new List<IndicatorListing>();

        // Add hardcoded indicators with base URL
        foreach (var indicator in IndicatorCatalog())
        {
            listing.Add(new IndicatorListing(baseEndpoint)
            {
                Name = indicator.Name,
                Uiid = indicator.Uiid,
                UiidEndpoint = indicator.UiidEndpoint,
                Category = indicator.Category,
                ChartType = indicator.ChartType,
                ChartConfig = indicator.ChartConfig,
                LegendOverride = indicator.LegendOverride,
                Order = indicator.Order,
                Parameters = indicator.Parameters,
                Results = indicator.Results
            });
        }

        return [.. listing.OrderBy(x => x.Name)];
    }
}
