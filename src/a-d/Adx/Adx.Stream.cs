namespace Skender.Stock.Indicators;

// AVERAGE DIRECTIONAL INDEX (STREAMING)

public partial class Adx
{
    // constructor
    public Adx(
        int lookbackPeriods)
    {
        ProtectedResults = [];
        LookbackPeriods = lookbackPeriods;
    }

    // PROPERTIES

    public IEnumerable<AdxResult> Results => ProtectedResults;
    internal List<AdxResult> ProtectedResults { get; set; }

    // configuration
    private int LookbackPeriods { get; set; }

    // carryover values
    private double PrevHigh { get; set; }
    private double PrevLow { get; set; }
    private double PrevClose { get; set; }

    private double PrevTrs { get; set; }
    private double PrevPdm { get; set; }
    private double PrevMdm { get; set; }
    private double PrevAdx { get; set; }

    // warmup values
    private double SumTr { get; set; }
    private double SumDx { get; set; }
}
