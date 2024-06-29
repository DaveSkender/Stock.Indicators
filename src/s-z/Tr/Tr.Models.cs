namespace Skender.Stock.Indicators;

public readonly record struct TrResult(
    DateTime Timestamp,
    double? Tr
) : IReusable
{
    double IReusable.Value => Tr.Null2NaN();
}
