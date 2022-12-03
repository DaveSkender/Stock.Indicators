namespace Skender.Stock.Indicators;

[Serializable]
public sealed class CmfResult : ResultBase, IReusableResult
{
    public CmfResult(DateTime date)
    {
        Date = date;
    }

    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double? Cmf { get; set; }

    double? IReusableResult.Value => Cmf;
}
