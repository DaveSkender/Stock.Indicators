namespace Skender.Stock.Indicators;

public sealed class AroonResult : ResultBase, IReusableResult
{
    public AroonResult(DateTime date)
    {
        Date = date;
    }

    public double? AroonUp { get; set; }
    public double? AroonDown { get; set; }
    public double? Oscillator { get; set; }

    double IReusableResult.Value => Oscillator.Null2NaN();
}
