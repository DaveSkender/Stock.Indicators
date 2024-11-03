namespace Skender.Stock.Indicators;

[Serializable]
public record ChandelierResult
(
    DateTime Timestamp,
    double? ChandelierExit
) : IReusable
{
    /// <inheritdoc/>
    public double Value => ChandelierExit.Null2NaN();
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
