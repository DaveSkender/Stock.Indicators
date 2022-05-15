namespace Skender.Stock.Indicators;

public interface IEmaResult : IResult
{
    public decimal? Ema { get; set; }
}

[Serializable]
public sealed class EmaResult : ResultBase, IEmaResult, IReusableResult
{
    public decimal? Ema { get; set; }

    double? IReusableResult.Value
    {
        get { return (double?)Ema; }
    }
}
