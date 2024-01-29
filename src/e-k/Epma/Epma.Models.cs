namespace Skender.Stock.Indicators;

public sealed record class EpmaResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Epma { get; set; }

    double IReusableResult.Value => Epma.Null2NaN();
}
