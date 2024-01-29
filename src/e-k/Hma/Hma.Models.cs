namespace Skender.Stock.Indicators;

public sealed record class HmaResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Hma { get; set; }

    double IReusableResult.Value => Hma.Null2NaN();
}
