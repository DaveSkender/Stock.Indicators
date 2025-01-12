/// <summary>
/// Represents the configuration of a chart.
/// </summary>
[Serializable]
public record class ChartConfig
{
    public double? MinimumYAxis { get; set; }
    public double? MaximumYAxis { get; set; }

    public ICollection<ChartThreshold>? Thresholds { get; set; }
}
