namespace Skender.Stock.Indicators;

public interface IEmaResult : IResult
{
    public double? Ema { get; set; }
}

[Serializable]
public sealed class EmaResult : ResultBase, IEmaResult, IReusableResult
{
    public double? Ema { get; set; }

    double? IReusableResult.Value
    {
        get { return Ema; }
    }
}
