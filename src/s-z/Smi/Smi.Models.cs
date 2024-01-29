namespace Skender.Stock.Indicators;

public sealed record class SmiResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Smi { get; set; }
    public double? Signal { get; set; }

    double IReusableResult.Value => Smi.Null2NaN();
}
