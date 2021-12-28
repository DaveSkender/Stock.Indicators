namespace Skender.Stock.Indicators;

[Serializable]
public class ChaikinOscResult : ResultBase
{
    public double MoneyFlowMultiplier { get; set; }
    public double MoneyFlowVolume { get; set; }
    public double Adl { get; set; }
    public double? Oscillator { get; set; }
}
