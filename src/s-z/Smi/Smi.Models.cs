namespace Skender.Stock.Indicators;

public readonly record struct SmiResult
(
    DateTime Timestamp,
    double? Smi,
    double? Signal
) : IReusable
{
    double IReusable.Value => Smi.Null2NaN();
}
