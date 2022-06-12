namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ParabolicSarResult : ResultBase, IReusableResult
{
    public decimal? Sar { get; set; }
    public bool? IsReversal { get; set; }

    double? IReusableResult.Value => (double?)Sar;
}
