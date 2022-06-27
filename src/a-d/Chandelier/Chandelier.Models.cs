namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ChandelierResult : ResultBase, IReusableResult
{
    public ChandelierResult(DateTime date)
    {
        Date = date;
    }

    public double? ChandelierExit { get; set; }

    double? IReusableResult.Value => ChandelierExit;
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
