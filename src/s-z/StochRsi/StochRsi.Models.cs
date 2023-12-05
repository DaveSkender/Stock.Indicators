namespace Skender.Stock.Indicators;

public sealed class StochRsiResult : ResultBase, IReusableResult
{
    public StochRsiResult(DateTime date)
    {
        Date = date;
    }

    public double? StochRsi { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => StochRsi.Null2NaN();
}
