namespace Skender.Stock.Indicators;

[Serializable]
public sealed class SuperTrendResult : ResultBase
{
    public SuperTrendResult(DateTime date)
    {
        Date = date;
    }

    public decimal? SuperTrend { get; set; }
    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }
}
