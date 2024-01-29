namespace Skender.Stock.Indicators;

public sealed record class BopResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Bop { get; set; }

    double IReusableResult.Value => Bop.Null2NaN();
}
