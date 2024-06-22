namespace Skender.Stock.Indicators;

public record struct TrResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Tr { get; set; }

    readonly double IReusableResult.Value
        => Tr.Null2NaN();
}
