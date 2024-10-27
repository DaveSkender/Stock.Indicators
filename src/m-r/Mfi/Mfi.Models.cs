namespace Skender.Stock.Indicators;

[Serializable]
public record MfiResult
(
    DateTime Timestamp,
    double? Mfi
) : IReusable
{
    public double Value => Mfi.Null2NaN();
}
