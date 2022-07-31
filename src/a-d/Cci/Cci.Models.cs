namespace Skender.Stock.Indicators;

[Serializable]
public sealed class CciResult : ResultBase, IReusableResult
{
    public CciResult(DateTime date)
    {
        Date = date;
    }

    public double? Cci { get; set; }

    double? IReusableResult.Value => Cci;
}
