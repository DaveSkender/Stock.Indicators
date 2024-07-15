namespace Skender.Stock.Indicators;

public record SmaResult(
    DateTime Timestamp,
    double? Sma
) : Reusable(Timestamp)
{
    public override double Value => Sma.Null2NaN();
}
