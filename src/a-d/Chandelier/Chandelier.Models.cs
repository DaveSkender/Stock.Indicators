namespace Skender.Stock.Indicators;

public sealed class ChandelierResult : ResultBase, IReusableResult
{
    public ChandelierResult(DateTime date)
    {
        Date = date;
    }

    public double? ChandelierExit { get; set; }

    double IReusableResult.Value => ChandelierExit.Null2NaN();
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
