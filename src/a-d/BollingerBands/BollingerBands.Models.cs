namespace Skender.Stock.Indicators;

public record struct BollingerBandsResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Sma { get; set; }
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    public double? PercentB { get; set; }
    public double? ZScore { get; set; }
    public double? Width { get; set; }

    readonly double IReusableResult.Value
        => PercentB.Null2NaN();
}
