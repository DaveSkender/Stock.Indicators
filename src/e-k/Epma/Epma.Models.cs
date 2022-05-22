namespace Skender.Stock.Indicators;

public interface IEpmaResult
{
    public double? Epma { get; }
}

[Serializable]
public sealed class EpmaResult : ResultBase, IEpmaResult, IReusableResult
{
    public double? Epma { get; set; }

    double? IReusableResult.Value => Epma;
}
