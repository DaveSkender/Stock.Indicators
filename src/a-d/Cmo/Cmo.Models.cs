namespace Skender.Stock.Indicators;

public record struct CmoResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Cmo { get; set; }

    readonly double IReusable.Value
        => Cmo.Null2NaN();
}
