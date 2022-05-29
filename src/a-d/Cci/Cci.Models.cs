namespace Skender.Stock.Indicators;

[Serializable]
public sealed class CciResult : ResultBase, IReusableResult
{
    public double? Cci { get; set; }

    double? IReusableResult.Value => Cci;
}
