namespace Skender.Stock.Indicators;

public record struct StdDevResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? StdDev { get; set; }
    public double? Mean { get; set; }
    public double? ZScore { get; set; }

    readonly double IReusableResult.Value
        => StdDev.Null2NaN();
}
