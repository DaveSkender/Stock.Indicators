namespace Skender.Stock.Indicators;

public sealed class RsiResult : ResultBase, IReusableResult
{
    public RsiResult(DateTime date)
    {
        Date = date;
    }

    public double? Rsi { get; set; }

    double IReusableResult.Value => Rsi.Null2NaN();
}
