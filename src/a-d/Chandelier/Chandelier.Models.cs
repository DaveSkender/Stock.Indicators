namespace Skender.Stock.Indicators;

public sealed record class ChandelierResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? ChandelierExit { get; set; }

    double IReusableResult.Value => ChandelierExit.Null2NaN();
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
