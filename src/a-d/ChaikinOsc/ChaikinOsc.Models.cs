namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ChaikinOscResult : ResultBase, IReusableResult
{
    public ChaikinOscResult(DateTime date)
    {
        Date = date;
    }

    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double? Adl { get; set; }
    public double? Oscillator { get; set; }

    double? IReusableResult.Value => Oscillator;
}
