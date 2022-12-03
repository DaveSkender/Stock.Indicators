namespace Skender.Stock.Indicators;

[Serializable]
public sealed class AwesomeResult : ResultBase, IReusableResult
{
    public AwesomeResult(DateTime date)
    {
        Date = date;
    }

    public double? Oscillator { get; set; }
    public double? Normalized { get; set; }

    double? IReusableResult.Value => Oscillator;
}
