namespace Skender.Stock.Indicators;

[Serializable]
public class VolatilityStopResult : ResultBase
{
    public decimal? Sar { get; set; }
    public bool? IsStop { get; set; }

    // SAR values as long/short stop bands
    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }
}
