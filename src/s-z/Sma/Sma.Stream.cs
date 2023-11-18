namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAMING)

public partial class Sma : TupleInTupleOut
{
    // constructor
    public Sma(
        TupleProvider provider,
        int lookbackPeriods)
    {
        TupleSupplier = provider;
        ProtectedResults = [];

        Initialize(lookbackPeriods);
    }

    // constructor for unmanaged flow
    public Sma(
        int lookbackPeriods)
    {
        ProtectedResults = [];

        Initialize(lookbackPeriods);
    }

    // PROPERTIES

    public IEnumerable<SmaResult> Results => ProtectedResults;
    internal List<SmaResult> ProtectedResults { get; set; }

    // configuration
    private int LookbackPeriods { get; set; }

    // METHODS

    // handle quote arrival
    public override void OnNext((Disposition , DateTime , double ) value)
        => Increment(value);

    // initialize and preload existing quote cache
    private void Initialize(int lookbackPeriods)
    {
        // also usable for reinitialization

        LookbackPeriods = lookbackPeriods;
        ProtectedResults = [];

        // TODO: should send delete instruction
        ProtectedTuples = [];

        // add from upstream cache
        List<(DateTime, double)> tuples = TupleSupplier.ProtectedTuples;

        for (int i = 0; i < tuples.Count; i++)
        {
            (DateTime date, double value) = tuples[i];
            Increment((Disposition.AddNew, date, value));
        }

        Subscribe();
    }
}
