namespace Skender.Stock.Indicators;

public interface IRsiResult : IResult
{
    public double? Rsi { get; set; }
}

[Serializable]
public sealed class RsiResult : ResultBase, IRsiResult, IReusableResult
{
    public double? Rsi { get; set; }

    double? IReusableResult.Value => Rsi;
}
