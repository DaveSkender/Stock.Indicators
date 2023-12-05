namespace Skender.Stock.Indicators;

public sealed class WilliamsResult : ResultBase, IReusableResult
{
    public WilliamsResult(DateTime date)
    {
        Date = date;
    }

    public double? WilliamsR { get; set; }

    double IReusableResult.Value => WilliamsR.Null2NaN();
}
