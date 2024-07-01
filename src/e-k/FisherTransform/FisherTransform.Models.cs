namespace Skender.Stock.Indicators;

public readonly record struct FisherTransformResult
(
    DateTime Timestamp,
    double? Fisher,
    double? Trigger
) : IReusable
{
    double IReusable.Value => Fisher.Null2NaN();
}
