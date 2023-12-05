namespace Skender.Stock.Indicators;

public sealed class BopResult : ResultBase, IReusableResult
{
    public BopResult(DateTime date)
    {
        Date = date;
    }

    public double? Bop { get; set; }

    double IReusableResult.Value => Bop.Null2NaN();
}
