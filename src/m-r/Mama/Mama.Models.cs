namespace Skender.Stock.Indicators;

public sealed record class MamaResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Mama { get; set; }
    public double? Fama { get; set; }

    double IReusableResult.Value => Mama.Null2NaN();
}
