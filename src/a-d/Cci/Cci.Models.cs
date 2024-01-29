namespace Skender.Stock.Indicators;

public sealed record class CciResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Cci { get; set; }

    double IReusableResult.Value => Cci.Null2NaN();
}
