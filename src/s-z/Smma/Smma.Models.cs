namespace Skender.Stock.Indicators;

public interface ISmmaResult
{
    public double? Smma { get; }
}

[Serializable]
public sealed class SmmaResult : ResultBase, ISmmaResult, IReusableResult
{
    public double? Smma { get; set; }

    double? IReusableResult.Value => Smma;
}
