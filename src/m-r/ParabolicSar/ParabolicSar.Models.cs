namespace Skender.Stock.Indicators;

public readonly record struct ParabolicSarResult
(
    DateTime Timestamp,
    double? Sar,
    bool? IsReversal
) : IReusable
{
    double IReusable.Value => Sar.Null2NaN();
}
