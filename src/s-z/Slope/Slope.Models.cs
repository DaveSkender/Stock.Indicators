namespace Skender.Stock.Indicators;

[Serializable]
public sealed class SlopeResult : ResultBase, IReusableResult
{
    public SlopeResult(DateTime date)
    {
        Date = date;
    }

    public double? Slope { get; set; }
    public double? Intercept { get; set; }
    public double? StdDev { get; set; }
    public double? RSquared { get; set; }
    public decimal? Line { get; set; } // last line segment only

    double? IReusableResult.Value => Slope;
}
