namespace Skender.Stock.Indicators;

public readonly record struct PvoResult
(
    DateTime Timestamp,
    double? Pvo,
    double? Signal,
    double? Histogram
) : IReusable
{
    double IReusable.Value => Pvo.Null2NaN();
}
