namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAMING)

public partial class Sma : ObsTupleSendTuple
{
    // constructor
    public Sma(
        TupleProvider provider,
        int lookbackPeriods)
    {
        TupleSupplier = provider;
        ProtectedResults = new();

        Initialize(lookbackPeriods);
    }

    public Sma(
        int lookbackPeriods)
    {
        TupleSupplier = new TupleProvider();
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
        if (TupleSupplier == null)
        {
            throw new ArgumentNullException(
                nameof(TupleSupplier),
                "Could not find data supplier.");
        }

        LookbackPeriods = lookbackPeriods;

        List<(DateTime, double)> tuples = TupleSupplier.ProtectedTuples;

        for (int i = 0; i < tuples.Count; i++)
        {
            Increment(tuples[i]);
        }

        Subscribe();
    }
}
