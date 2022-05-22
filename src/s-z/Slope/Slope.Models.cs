namespace Skender.Stock.Indicators;

public interface ISlopeResult
{
    public double? Slope { get; }
    public double? Intercept { get; }
    public decimal? Line { get; }
}

[Serializable]
public sealed class SlopeResult : ResultBase, ISlopeResult, IReusableResult
{
    public double? Slope { get; set; }
    public double? Intercept { get; set; }
    public double? StdDev { get; set; }
    public double? RSquared { get; set; }
    public decimal? Line { get; set; } // last line segment only

    double? IReusableResult.Value => Slope;
}
