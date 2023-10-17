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

        LookbackPeriods = lookbackPeriods;

        Initialize();
    }

    // PROPERTIES

    public IEnumerable<SmaResult> Results => ProtectedResults;
    internal List<SmaResult> ProtectedResults { get; set; }

    private int LookbackPeriods { get; set; }

    // METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Add(value);

    // add new tuple quote
    internal void Add((DateTime Date, double Value) tp)
    {
        if (Supplier == null)
        {
            throw new ArgumentNullException(nameof(Supplier), "Could not find data source.");
        }

        // candidate result (empty)
        SmaResult r = new(tp.Date);

        // initialize
        int length = ProtectedResults.Count;

        // handle first value
        if (length == 0)
        {
            ProtectedResults.Add(r);
            SendToChain(r);
            return;
        }

        // calculate incremental value
        // TODO: this index value won't work for late arrivals
        r.Sma = Increment(
            Supplier.ProtectedTuples,
            length,
            LookbackPeriods)
            .NaN2Null();

        // last entry
        SmaResult last = ProtectedResults[length - 1];

        // add new
        if (r.Date > last.Date)
        {
            ProtectedResults.Add(r);
            SendToChain(r);
        }

        // update last
        else if (r.Date == last.Date)
        {
            last.Sma = r.Sma;
            SendToChain(last);
        }

        // late arrival
        else
        {
            // heal
            throw new NotImplementedException();

            // existing and index in sync?

            // new and index otherwise in sync?

            // all other scenarios: unsubscribe from provider and end transmission to others?
        }
    }

    // initialize with existing quote cache
    private void Initialize()
    {
        if (Supplier != null)
        {
            List<(DateTime, double)> tuples = Supplier
                .ProtectedTuples;

            for (int i = 0; i < tuples.Count; i++)
            {
                Add(tuples[i]);
            }

            Subscribe();
        }
    }
}
