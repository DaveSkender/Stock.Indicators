namespace Skender.Stock.Indicators;

public sealed record class TrResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Tr { get; set; }

    double IReusableResult.Value => Tr.Null2NaN();
}
