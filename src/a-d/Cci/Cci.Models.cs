namespace Skender.Stock.Indicators;

public record struct CciResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Cci { get; set; }

    readonly double IReusableResult.Value
        => Cci.Null2NaN();
}
