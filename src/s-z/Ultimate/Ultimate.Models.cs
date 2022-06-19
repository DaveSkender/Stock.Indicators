namespace Skender.Stock.Indicators;

[Serializable]
public sealed class UltimateResult : ResultBase, IReusableResult
{
    public double? Ultimate { get; set; }

    double? IReusableResult.Value => Ultimate;
}
