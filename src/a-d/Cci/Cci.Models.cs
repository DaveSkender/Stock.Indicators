namespace Skender.Stock.Indicators;

public record struct CciResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Cci { get; set; }

    readonly double IReusable.Value
        => Cci.Null2NaN();
}
