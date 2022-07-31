namespace Skender.Stock.Indicators;

[Serializable]
public sealed class FcbResult : ResultBase
{
    public FcbResult(DateTime date)
    {
        Date = date;
    }

    public decimal? UpperBand { get; set; }
    public decimal? LowerBand { get; set; }
}
