namespace Skender.Stock.Indicators;

public sealed record class T3Result : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? T3 { get; set; }

    double IReusableResult.Value => T3.Null2NaN();
}
