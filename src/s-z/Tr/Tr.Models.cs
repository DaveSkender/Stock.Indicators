namespace Skender.Stock.Indicators;

[Serializable]
public class TrResult : ResultBase, IReusableResult
{
    public TrResult(DateTime date)
    {
        Date = date;
    }

    public double? Tr { get; set; }

    double? IReusableResult.Value => Tr;
}
