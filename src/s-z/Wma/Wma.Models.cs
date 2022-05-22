namespace Skender.Stock.Indicators;

public interface IWmaResult
{
    public double? Wma { get; }
}

[Serializable]
public sealed class WmaResult : ResultBase, IWmaResult, IReusableResult
{
    public double? Wma { get; set; }

    double? IReusableResult.Value => Wma;
}
