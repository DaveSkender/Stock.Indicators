namespace Skender.Stock.Indicators;

public interface IEmaResult : IResult
{
    public double? Ema { get; }
}

[Serializable]
public sealed class EmaResult : ResultBase, IEmaResult, IReusableResult
{
    public double? Ema { get; set; }

    double? IReusableResult.Value => Ema;
}
