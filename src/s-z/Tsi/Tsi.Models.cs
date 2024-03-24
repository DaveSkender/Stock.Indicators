namespace Skender.Stock.Indicators;

public sealed record class TsiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Tsi { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Tsi.Null2NaN();
}
