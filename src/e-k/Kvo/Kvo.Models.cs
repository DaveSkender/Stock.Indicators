namespace Skender.Stock.Indicators;

[Serializable]
public class KvoResult : ResultBase, IReusableResult
{
    internal KvoResult(DateTime date)
    {
        Date = date;
    }

    public double? Oscillator { get; set; }
    public double? Signal { get; set; }

    double? IReusableResult.Value => Oscillator;
}
