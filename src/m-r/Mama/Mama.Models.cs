namespace Skender.Stock.Indicators;

[Serializable]
public sealed class MamaResult : ResultBase, IReusableResult
{
    public double? Mama { get; set; }
    public double? Fama { get; set; }

    double? IReusableResult.Value => Mama;
}
