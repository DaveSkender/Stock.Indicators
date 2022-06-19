namespace Skender.Stock.Indicators;

[Serializable]
public sealed class TemaResult : ResultBase, IReusableResult
{
    public double? Tema { get; set; }

    double? IReusableResult.Value => Tema;
}
