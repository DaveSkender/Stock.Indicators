namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

public partial class Ema : ChainProvider
{
    // constructor
    public Ema(
        TupleProvider provider,
        int lookbackPeriods)
    {
        Supplier = provider;
        ProtectedResults = new();

        Initialize(lookbackPeriods);
    }

    public Ema(
        int lookbackPeriods)
    {
        Supplier = new TupleProvider();
        ProtectedResults = new();
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

    // METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Increment(value);

    // initialize and preload existing quote cache
    private void Initialize(int lookbackPeriods)
    {
        if (Supplier == null)
        {
            throw new ArgumentNullException(
                nameof(Supplier),
                "Could not find data supplier.");
        }

        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        SumValue = 0;

        List<(DateTime, double)> tuples = Supplier
            .ProtectedTuples;

        for (int i = 0; i < tuples.Count; i++)
        {
            Increment(tuples[i]);
        }

        Subscribe();
    }
}
