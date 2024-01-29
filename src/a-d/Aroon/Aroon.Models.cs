namespace Skender.Stock.Indicators;

public sealed record class AroonResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? AroonUp { get; set; }
    public double? AroonDown { get; set; }
    public double? Oscillator { get; set; }

    double IReusableResult.Value => Oscillator.Null2NaN();
}
