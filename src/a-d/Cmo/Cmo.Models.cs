namespace Skender.Stock.Indicators;

public sealed record class CmoResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Cmo { get; set; }

    double IReusableResult.Value => Cmo.Null2NaN();
}
