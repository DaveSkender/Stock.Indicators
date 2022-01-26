namespace Skender.Stock.Indicators;

[Serializable]
public class BollingerBandsResult : ResultBase
{
    public decimal? Sma { get; set; }
    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }

    public double? PercentB { get; set; }
    public double? ZScore { get; set; }
    public double? Width { get; set; }
}
