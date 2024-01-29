namespace Skender.Stock.Indicators;

public sealed record class HurstResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? HurstExponent { get; set; }

    double IReusableResult.Value => HurstExponent.Null2NaN();
}
