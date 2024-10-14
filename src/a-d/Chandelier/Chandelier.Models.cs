namespace Skender.Stock.Indicators;

public record ChandelierResult
(
    DateTime Timestamp,
    double? ChandelierExit
) : IReusable
{
    public double Value => ChandelierExit.Null2NaN();
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
