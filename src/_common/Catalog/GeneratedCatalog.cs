namespace Skender.Stock.Indicators;

/// <summary>
/// Auto-generated catalog of all indicators in the library.
/// This is a placeholder class that will be replaced by the source generator.
/// </summary>
public static partial class GeneratedCatalog
{
    /// <summary>
    /// Gets the complete list of indicators
    /// </summary>
    public static IReadOnlyList<IndicatorListing> Indicators
        => GeneratedIndicators;

    /// <summary>
    /// Gets the auto-generated list of indicators
    /// This is a stub that will be replaced by the source generator.
    /// </summary>
    public static IReadOnlyList<IndicatorListing> GeneratedIndicators
        => new List<IndicatorListing>
        {
            // Generated test indicator for testing purposes
            new() {
                Name = "Generated Test Indicator",
                Uiid = "GEN_TEST",
                Category = Category.PriceTrend,
                ChartType = ChartType.Overlay,
                Parameters =
                [
                    new IndicatorParamConfig
                    {
                        DisplayName = "Lookback Periods",
                        ParamName = "lookbackPeriods",
                        DataType = "int",
                        DefaultValue = 14,
                        Minimum = 1,
                        Maximum = 100
                    }
                ],
                Results =
                [
                    new IndicatorResultConfig
                    {
                        DisplayName = "Generated Value",
                        TooltipTemplate = "GEN([P1])",
                        DataName = "gen",
                        DataType = "number",
                        LineType = "solid",
                        DefaultColor = ChartColors.StandardGreen
                    }
                ]
            },

            // Adding Stream-based indicator
            new() {
                Name = "Data Stream Analyzer",
                Uiid = "STREAM_TEST",
                Category = Category.Oscillator,
                ChartType = ChartType.Oscillator,
                Parameters =
                [
                    new IndicatorParamConfig
                    {
                        DisplayName = "Window Size",
                        ParamName = "windowSize",
                        DataType = "int",
                        DefaultValue = 10,
                        Minimum = 2,
                        Maximum = 50
                    }
                ],
                Results =
                [
                    new IndicatorResultConfig
                    {
                        DisplayName = "Stream Value",
                        TooltipTemplate = "STR([P1])",
                        DataName = "stream",
                        DataType = "number",
                        LineType = "dotted",
                        DefaultColor = ChartColors.StandardRed
                    }
                ]
            },

            // Adding Buffer-based indicator
            new() {
                Name = "Data Buffer Handler",
                Uiid = "BUFFER_TEST",
                Category = Category.VolumeBased,
                ChartType = ChartType.Oscillator,
                Parameters =
                [
                    new IndicatorParamConfig
                    {
                        DisplayName = "Buffer Size",
                        ParamName = "bufferSize",
                        DataType = "int",
                        DefaultValue = 25,
                        Minimum = 5,
                        Maximum = 100
                    }
                ],
                Results =
                [
                    new IndicatorResultConfig
                    {
                        DisplayName = "Buffer Value",
                        TooltipTemplate = "BUF([P1])",
                        DataName = "buffer",
                        DataType = "number",
                        LineType = "dashed",
                        DefaultColor = ChartColors.StandardPurple
                    }
                ]
            },

            // Adding Series-based indicator
            new() {
                Name = "Time Series Analyzer",
                Uiid = "SERIES_TEST",
                Category = Category.MovingAverage,
                ChartType = ChartType.Overlay,
                Parameters =
                [
                    new IndicatorParamConfig
                    {
                        DisplayName = "Series Length",
                        ParamName = "seriesLength",
                        DataType = "int",
                        DefaultValue = 30,
                        Minimum = 10,
                        Maximum = 200
                    }
                ],
                Results =
                [
                    new IndicatorResultConfig
                    {
                        DisplayName = "Series Value",
                        TooltipTemplate = "SER([P1])",
                        DataName = "series",
                        DataType = "number",
                        LineType = "solid",
                        DefaultColor = ChartColors.StandardOrange
                    }
                ]
            }
        };
}
