namespace Skender.Stock.Indicators;

[Serializable]
public record BopResult
(
    DateTime Timestamp,
    double? Bop
) : IReusable
{
    public double Value => Bop.Null2NaN();
}
