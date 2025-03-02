/// <summary>
/// Represents the configuration for a chart, including Y-axis limits and thresholds.
/// </summary>
[Serializable]
public record ChartConfig
{
    /// <summary>
    /// Gets or sets the minimum value for the Y-axis.
    /// </summary>
    public double? MinimumYAxis { get; init; }

    /// <summary>
    /// Gets or sets the maximum value for the Y-axis.
    /// </summary>
    public double? MaximumYAxis { get; init; }

    /// <summary>
    /// Gets or sets the collection of thresholds for the chart.
    /// </summary>
    public ICollection<ChartThreshold>? Thresholds { get; init; }
}
