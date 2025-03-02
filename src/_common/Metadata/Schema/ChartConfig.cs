[Serializable]
public record ChartConfig
{
    public double? MinimumYAxis { get; }
    public double? MaximumYAxis { get; }

    public ICollection<ChartThreshold>? Thresholds { get; }
}
