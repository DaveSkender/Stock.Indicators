namespace Skender.Stock.Indicators;

public record SmiResult
(
    DateTime Timestamp,
    double? Smi,
    double? Signal
) : Reusable(Timestamp)
{
    public override double Value => Smi.Null2NaN();
}
