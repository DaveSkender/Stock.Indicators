namespace Skender.Stock.Indicators;

public sealed record class VwmaResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Vwma { get; set; }

    double IReusableResult.Value => Vwma.Null2NaN();
}
