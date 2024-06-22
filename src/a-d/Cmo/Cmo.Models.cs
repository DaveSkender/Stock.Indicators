namespace Skender.Stock.Indicators;

public record struct CmoResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Cmo { get; set; }

    readonly double IReusableResult.Value
        => Cmo.Null2NaN();
}
