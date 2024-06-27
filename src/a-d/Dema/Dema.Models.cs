namespace Skender.Stock.Indicators;

public record struct DemaResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Dema { get; set; }

    readonly double IReusable.Value
        => Dema.Null2NaN();
}
