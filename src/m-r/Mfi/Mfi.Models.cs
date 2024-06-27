namespace Skender.Stock.Indicators;

public record struct MfiResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Mfi { get; set; }

    readonly double IReusable.Value
        => Mfi.Null2NaN();
}
