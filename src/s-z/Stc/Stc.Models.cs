namespace Skender.Stock.Indicators;

public record struct StcResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Stc { get; set; }

    readonly double IReusableResult.Value
        => Stc.Null2NaN();
}
