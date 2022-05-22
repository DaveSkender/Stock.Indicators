namespace Skender.Stock.Indicators;

public interface IDpoResult : IResult
{
    public double? Dpo { get; }
}

[Serializable]
public sealed class DpoResult : ResultBase, IDpoResult, IReusableResult
{
    public double? Sma { get; set; }
    public double? Dpo { get; set; }

    double? IReusableResult.Value => Dpo;
}
