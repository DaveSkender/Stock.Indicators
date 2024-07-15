namespace Skender.Stock.Indicators;

public record RsiResult
(
    DateTime Timestamp,
    double? Rsi = null
) : Reusable(Timestamp)
{
    public override double Value => Rsi.Null2NaN();
}
