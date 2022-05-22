namespace Skender.Stock.Indicators;

public interface IHmaResult
{
    public double? Hma { get; }
}

[Serializable]
public sealed class HmaResult : ResultBase, IHmaResult, IReusableResult
{
    public double? Hma { get; set; }

    double? IReusableResult.Value => Hma;
}
