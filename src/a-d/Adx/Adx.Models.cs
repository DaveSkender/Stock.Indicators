namespace Skender.Stock.Indicators;

[Serializable]
public sealed class AdxResult : ResultBase, IReusableResult
{
    public AdxResult(DateTime date)
    {
        Date = date;
    }

    public double? Pdi { get; set; }
    public double? Mdi { get; set; }
    public double? Adx { get; set; }
    public double? Adxr { get; set; }

    double? IReusableResult.Value => Adx;
}

/// <include file='./info.xml' path='info/type[@name="input"]/*' />
///
public class AdxInput
{
    public AdxInput(
        double initialHigh,
        double initialLow,
        double initialClose)
    {
        PrevHigh = High = initialHigh;
        PrevLow = Low = initialLow;
        PrevClose = Close = initialClose;

        WindowAdx = double.NaN;

        PrevTrs = 0;
        PrevPdm = 0;
        PrevMdm = 0;
        PrevAdx = 0;

        SumTr = 0;
        SumDx = 0;
    }

    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }

    public double WindowAdx { get; set; }

    internal double PrevHigh { get; set; }
    internal double PrevLow { get; set; }
    internal double PrevClose { get; set; }

    internal double PrevTrs { get; set; }
    internal double PrevPdm { get; set; }
    internal double PrevMdm { get; set; }
    internal double PrevAdx { get; set; }

    internal double SumTr { get; set; }
    internal double SumDx { get; set; }
}
