namespace Skender.Stock.Indicators;

public sealed record class MfiResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Mfi { get; set; }

    double IReusableResult.Value => Mfi.Null2NaN();
}
