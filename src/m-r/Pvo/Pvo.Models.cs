namespace Skender.Stock.Indicators;

public record PvoResult
(
    DateTime Timestamp,
    double? Pvo,
    double? Signal,
    double? Histogram
) : Reusable(Timestamp)
{
    public override double Value => Pvo.Null2NaN();
}
