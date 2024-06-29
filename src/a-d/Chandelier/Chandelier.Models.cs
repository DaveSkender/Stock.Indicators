namespace Skender.Stock.Indicators;

public readonly record struct ChandelierResult
(
    DateTime Timestamp,
    double? ChandelierExit
) : IReusable
{
    double IReusable.Value => ChandelierExit.Null2NaN();
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
