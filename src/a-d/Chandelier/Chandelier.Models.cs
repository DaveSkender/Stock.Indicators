namespace Skender.Stock.Indicators;

public record struct ChandelierResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? ChandelierExit { get; set; }

    readonly double IReusableResult.Value
        => ChandelierExit.Null2NaN();
}

public enum ChandelierType
{
    Long = 0,
    Short = 1
}
