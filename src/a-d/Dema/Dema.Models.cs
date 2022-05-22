namespace Skender.Stock.Indicators;

public interface IDemaResult : IResult
{
    public double? Dema { get; }
}

[Serializable]
public sealed class DemaResult : ResultBase, IDemaResult, IReusableResult
{
    public double? Dema { get; set; }

    double? IReusableResult.Value => Dema;
}
