namespace Skender.Stock.Indicators;

public record struct TrResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Tr { get; set; }

    readonly double IReusable.Value
        => Tr.Null2NaN();
}
