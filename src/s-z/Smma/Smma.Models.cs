namespace Skender.Stock.Indicators;

public sealed record class SmmaResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Smma { get; set; }

    double IReusableResult.Value => Smma.Null2NaN();
}
