namespace Skender.Stock.Indicators;

[Serializable]
public sealed class CorrResult : ResultBase, IReusableResult
{
    public CorrResult(DateTime date)
    {
        Date = date;
    }

    public double? VarianceA { get; set; }
    public double? VarianceB { get; set; }
    public double? Covariance { get; set; }
    public double? Correlation { get; set; }
    public double? RSquared { get; set; }

    double? IReusableResult.Value => Correlation;
}
