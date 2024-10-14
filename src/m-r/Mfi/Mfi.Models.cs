namespace Skender.Stock.Indicators;

public record MfiResult
(
    DateTime Timestamp,
    double? Mfi
) : IReusable
{
    public double Value => Mfi.Null2NaN();
}
