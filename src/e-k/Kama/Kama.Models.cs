namespace Skender.Stock.Indicators;

public sealed record class KamaResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? ER { get; set; }
    public double? Kama { get; set; }

    double IReusableResult.Value => Kama.Null2NaN();
}
