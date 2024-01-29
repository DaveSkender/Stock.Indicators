namespace Skender.Stock.Indicators;

public sealed record class DemaResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Dema { get; set; }

    double IReusableResult.Value => Dema.Null2NaN();
}
