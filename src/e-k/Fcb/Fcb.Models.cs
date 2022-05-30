namespace Skender.Stock.Indicators;

[Serializable]
public sealed class FcbResult : ResultBase
{
    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }
}
