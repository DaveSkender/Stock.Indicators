namespace Skender.Stock.Indicators;

public sealed class FcbResult : ResultBase
{
    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }
}
