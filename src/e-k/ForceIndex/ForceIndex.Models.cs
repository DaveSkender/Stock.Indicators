namespace Skender.Stock.Indicators;

public readonly record struct ForceIndexResult
(
    DateTime Timestamp,
    double? ForceIndex
) : IReusable
{
    double IReusable.Value => ForceIndex.Null2NaN();
}
