namespace Skender.Stock.Indicators;

public record struct MfiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Mfi { get; set; }

    readonly double IReusableResult.Value
        => Mfi.Null2NaN();
}
