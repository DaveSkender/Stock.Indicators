namespace Skender.Stock.Indicators;

public sealed record class DpoResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Sma { get; set; }
    public double? Dpo { get; set; }

    double IReusableResult.Value => Dpo.Null2NaN();
}
