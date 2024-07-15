namespace Skender.Stock.Indicators;

public record ChandelierResult
(
    DateTime Timestamp,
    double? ChandelierExit
) : Reusable(Timestamp)
{
    public override double Value => ChandelierExit.Null2NaN();
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
