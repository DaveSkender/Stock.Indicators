namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ParabolicSarResult : ResultBase, IReusableResult
{
    public double? Sar { get; set; }
    public bool? IsReversal { get; set; }

    double? IReusableResult.Value => Sar;
}
