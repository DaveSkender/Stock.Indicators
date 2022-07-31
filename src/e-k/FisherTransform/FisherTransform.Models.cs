namespace Skender.Stock.Indicators;

[Serializable]
public sealed class FisherTransformResult : ResultBase, IReusableResult
{
    public FisherTransformResult(DateTime date)
    {
        Date = date;
    }

    public double? Fisher { get; set; }
    public double? Trigger { get; set; }

    double? IReusableResult.Value => Fisher;
}
