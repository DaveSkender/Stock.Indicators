namespace Skender.Stock.Indicators;

public sealed class ForceIndexResult : ResultBase, IReusableResult
{
    public ForceIndexResult(DateTime date)
    {
        Date = date;
    }

    public double? ForceIndex { get; set; }

    double IReusableResult.Value => ForceIndex.Null2NaN();
}
