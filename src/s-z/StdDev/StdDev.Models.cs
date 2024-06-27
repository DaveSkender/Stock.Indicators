namespace Skender.Stock.Indicators;

public record struct StdDevResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? StdDev { get; set; }
    public double? Mean { get; set; }
    public double? ZScore { get; set; }

    readonly double IReusable.Value
        => StdDev.Null2NaN();
}
