namespace Skender.Stock.Indicators;

public sealed class DynamicResult : ResultBase, IReusableResult
{
    public DynamicResult(DateTime date)
    {
        Date = date;
    }

    public double? Dynamic { get; set; }

    double IReusableResult.Value => Dynamic.Null2NaN();
}
