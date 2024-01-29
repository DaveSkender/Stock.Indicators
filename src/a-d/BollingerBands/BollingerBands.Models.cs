namespace Skender.Stock.Indicators;

public sealed record class BollingerBandsResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? Sma { get; set; }
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    public double? PercentB { get; set; }
    public double? ZScore { get; set; }
    public double? Width { get; set; }

    double IReusableResult.Value => PercentB.Null2NaN();
}
