namespace Skender.Stock.Indicators;

public record SmiResult
(
    DateTime Timestamp,
    double? Smi,
    double? Signal
) : IReusable
{
    public double Value => Smi.Null2NaN();
}
