namespace Skender.Stock.Indicators;

public sealed class TrResult : ResultBase, IReusableResult
{
    public TrResult(DateTime date)
    {
        Date = date;
    }

    public double? Tr { get; set; }

    double IReusableResult.Value => Tr.Null2NaN();
}
