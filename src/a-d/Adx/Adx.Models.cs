namespace Skender.Stock.Indicators;

public record AdxResult
(
    DateTime Timestamp,
    double? Pdi = null,
    double? Mdi = null,
    double? Adx = null,
    double? Adxr = null
) : IReusable
{
    public double Value => Adx.Null2NaN();
}
