namespace Skender.Stock.Indicators;

public record TrResult(
    DateTime Timestamp,
    double? Tr
) : Reusable(Timestamp)
{
    public override double Value => Tr.Null2NaN();
}
