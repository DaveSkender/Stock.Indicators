namespace Skender.Stock.Indicators;

[Serializable]
public sealed class BollingerBandsResult : ResultBase, IReusableResult
{
    public BollingerBandsResult(DateTime date)
    {
        Date = date;
    }

    public double? Sma { get; set; }
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }

    public double? PercentB { get; set; }
    public double? ZScore { get; set; }
    public double? Width { get; set; }

    double? IReusableResult.Value => PercentB;
}
