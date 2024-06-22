namespace Skender.Stock.Indicators;

public record struct MamaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Mama { get; set; }
    public double? Fama { get; set; }

    readonly double IReusableResult.Value
        => Mama.Null2NaN();
}
