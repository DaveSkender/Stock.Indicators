namespace Skender.Stock.Indicators;

public record EpmaResult
(
    DateTime Timestamp,
    double? Epma
) : Reusable(Timestamp)
{
    public override double Value => Epma.Null2NaN();
}
