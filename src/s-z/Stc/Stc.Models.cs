namespace Skender.Stock.Indicators;

public record struct StcResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Stc { get; set; }

    readonly double IReusable.Value
        => Stc.Null2NaN();
}
