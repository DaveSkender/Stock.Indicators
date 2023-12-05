namespace Skender.Stock.Indicators;

public sealed class ChopResult : ResultBase, IReusableResult
{
    public ChopResult(DateTime date)
    {
        Date = date;
    }

    public double? Chop { get; set; }

    double IReusableResult.Value => Chop.Null2NaN();
}
