namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAMING)

public partial class Sma : ChainProvider
{
    // constructor
    public Sma(
        TupleProvider provider,
        int lookbackPeriods)
    {
        Supplier = provider;
        ProtectedResults = new();

        Initialize(lookbackPeriods);
    }

    public Sma(
        int lookbackPeriods)
    {
        Supplier = new TupleProvider();
        ProtectedResults = new();

        Initialize(lookbackPeriods);
    }

    // PROPERTIES

    public IEnumerable<SmaResult> Results => ProtectedResults;
    internal List<SmaResult> ProtectedResults { get; set; }

    // configuration
    private int LookbackPeriods { get; set; }

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

        List<(DateTime, double)> tuples = Supplier
                .ProtectedTuples;

        for (int i = 0; i < tuples.Count; i++)
        {
            Increment(tuples[i]);
        }

        Subscribe();
    }
}
