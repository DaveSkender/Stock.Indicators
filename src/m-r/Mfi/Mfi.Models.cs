namespace Skender.Stock.Indicators;

public record MfiResult
(
    DateTime Timestamp,
    double? Mfi
) : Reusable(Timestamp)
{
    public override double Value => Mfi.Null2NaN();
}
