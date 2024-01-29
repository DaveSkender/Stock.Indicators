namespace Skender.Stock.Indicators;

public sealed record class StochRsiResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? StochRsi { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => StochRsi.Null2NaN();
}
