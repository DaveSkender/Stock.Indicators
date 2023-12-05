namespace Skender.Stock.Indicators;

public sealed class VortexResult : ResultBase
{
    public VortexResult(DateTime date)
    {
        Date = date;
    }

    public double? Pvi { get; set; }
    public double? Nvi { get; set; }
}
