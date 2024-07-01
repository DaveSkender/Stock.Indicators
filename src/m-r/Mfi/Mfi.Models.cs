namespace Skender.Stock.Indicators;

public readonly record struct MfiResult
(
    DateTime Timestamp,
    double? Mfi
) : IReusable
{
    double IReusable.Value => Mfi.Null2NaN();
}
