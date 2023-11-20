namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

public partial class Ema : TupleInTupleOut
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

    // TODO: make generic TResult storage
    public IEnumerable<EmaResult> Results => ProtectedResults;
    internal List<EmaResult> ProtectedResults { get; set; }

    // configuration
    private int LookbackPeriods { get; set; }
    private double K { get; set; }

    // METHODS

    // handle quote arrival
    public override void OnNext((Act, DateTime, double) value) => Increment(value);

    // initialize and preload existing quote cache
    private void Initialize(int lookbackPeriods)
    {
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        ResetTupleCache();
        ResetResultCache();

        Subscribe();
    }

    // reset and reinitialize cache
    // TODO: make generic for TResult or make Interface for indicators
    private void ResetResultCache()
    {
        ProtectedResults = [];

        // add from upstream cache
        List<(DateTime, double)> tuples = TupleSupplier.ProtectedTuples;

        for (int i = 0; i < tuples.Count; i++)
        {
            (DateTime date, double value) = tuples[i];
            Increment((Act.AddNew, date, value));
        }
    }
}
