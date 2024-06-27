namespace Skender.Stock.Indicators;

public record struct MamaResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Mama { get; set; }
    public double? Fama { get; set; }

    readonly double IReusable.Value
        => Mama.Null2NaN();
}
