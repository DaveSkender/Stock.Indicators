namespace Skender.Stock.Indicators;

[Serializable]
public sealed class MfiResult : ResultBase, IReusableResult
{
    public double? Mfi { get; set; }

    double? IReusableResult.Value => Mfi;
}
