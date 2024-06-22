namespace Skender.Stock.Indicators;

public record struct DemaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Dema { get; set; }

    readonly double IReusableResult.Value
        => Dema.Null2NaN();
}
