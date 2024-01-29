namespace Skender.Stock.Indicators;

public sealed record class PmoResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Pmo { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Pmo.Null2NaN();
}
