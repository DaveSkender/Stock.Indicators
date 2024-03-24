namespace Skender.Stock.Indicators;

public sealed record class ChopResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Chop { get; set; }

    double IReusableResult.Value => Chop.Null2NaN();
}
