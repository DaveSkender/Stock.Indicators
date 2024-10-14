namespace Skender.Stock.Indicators;

public record TrResult(
    DateTime Timestamp,
    double? Tr
) : IReusable
{
    public double Value => Tr.Null2NaN();
}
