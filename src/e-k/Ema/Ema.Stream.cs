namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

public partial class Ema : ObsTupleSendTuple
{
    // constructor for tuple flow
    public Ema(
        TupleProvider provider,
        int lookbackPeriods)
    {
        TupleSupplier = provider;
        ProtectedResults = [];

        Initialize(lookbackPeriods);
    }

    // constructor for unmanaged flow
    public Ema(
        int lookbackPeriods)
    {
        ProtectedResults = [];

        Initialize(lookbackPeriods);
    }

    // PROPERTIES

    public IEnumerable<EmaResult> Results => ProtectedResults;
    internal List<EmaResult> ProtectedResults { get; set; }

    // configuration
    private int LookbackPeriods { get; set; }
    private double K { get; set; }

    // warmup values
    private double SumValue { get; set; }

    // private bool IsWarmup { get; set; }

    // METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Increment(value);

    // initialize and preload existing quote cache
    private void Initialize(int lookbackPeriods)
    {
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        SumValue = 0;

        List<(DateTime, double)> tuples = TupleSupplier.ProtectedTuples;

        for (int i = 0; i < tuples.Count; i++)
        {
            Increment(tuples[i]);
        }

        Subscribe();
    }
}
