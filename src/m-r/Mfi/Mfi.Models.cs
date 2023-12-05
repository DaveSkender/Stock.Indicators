namespace Skender.Stock.Indicators;

public sealed class MfiResult : ResultBase, IReusableResult
{
    public MfiResult(DateTime date)
    {
        Date = date;
    }

    public double? Mfi { get; set; }

    double IReusableResult.Value => Mfi.Null2NaN();
}
