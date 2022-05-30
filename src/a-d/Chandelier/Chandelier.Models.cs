namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ChandelierResult : ResultBase, IReusableResult
{
    public decimal? ChandelierExit { get; set; }

    double? IReusableResult.Value => (double?)ChandelierExit;
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
