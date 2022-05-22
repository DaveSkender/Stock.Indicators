namespace Skender.Stock.Indicators;

public interface IAlmaResult
{
    public double? Alma { get; }
}

[Serializable]
public sealed class AlmaResult : ResultBase, IAlmaResult, IReusableResult
{
    public double? Alma { get; set; }

    double? IReusableResult.Value => Alma;
}
