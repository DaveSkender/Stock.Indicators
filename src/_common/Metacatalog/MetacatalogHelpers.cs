namespace Skender.Stock.Indicators;

/// <summary>
/// Provides helper methods for generating chart configurations and indicator result configurations.
/// </summary>
internal static class MetacatalogHelpers
{
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
}

