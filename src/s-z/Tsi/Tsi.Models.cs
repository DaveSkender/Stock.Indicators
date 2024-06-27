namespace Skender.Stock.Indicators;

public record struct TsiResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Tsi { get; set; }
    public double? Signal { get; set; }

    readonly double IReusable.Value
        => Tsi.Null2NaN();
}
