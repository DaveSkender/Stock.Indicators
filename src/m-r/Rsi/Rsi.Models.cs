namespace Skender.Stock.Indicators;

public sealed record class RsiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Rsi { get; set; }

    double IReusableResult.Value => Rsi.Null2NaN();
}
