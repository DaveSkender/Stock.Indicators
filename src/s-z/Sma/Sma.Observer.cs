namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAMING)

public class SmaObserver : ChainProvider
{
    public SmaObserver(
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

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }

    // increment calculation
    internal static double Increment(
        List<(DateTime Date, double Value)> values,
        int index,
        int lookbackPeriods)
    {
        if (index < lookbackPeriods - 1)
        {
            return double.NaN;
        }

        double sum = 0;
        for (int i = index - lookbackPeriods + 1; i <= index; i++)
        {
            sum += values[i].Value;
        }

        return sum / lookbackPeriods;
    }

    // NON-STATIC METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Add(value);

    // add new tuple quote
    internal void Add((DateTime Date, double Value) tp)
    {
        if (Supplier == null)
        {
            throw new ArgumentNullException(nameof(Supplier), "Could not find data source.");
        }

        // candidate result
        SmaResult r = new(tp.Date);

        // initialize
        int lengthRes = ProtectedResults.Count;
        int lengthSrc = Supplier.ProtectedTuples.Count;

        // handle first value
        if (lengthRes == 0)
        {
            ProtectedResults.Add(r);
            SendToChain(r);
            return;
        }

        SmaResult lastResult = ProtectedResults[lengthRes - 1];
        (DateTime lastSrcDate, double _) = Supplier.ProtectedTuples[lengthSrc - 1];

        if (r.Date == lastSrcDate)
        {
            r.Sma = Increment(
                Supplier.ProtectedTuples,
                lengthSrc - 1,
                LookbackPeriods)
                .NaN2Null();
        }

        // add new
        if (r.Date > lastResult.Date)
        {
            ProtectedResults.Add(r);
            SendToChain(r);
        }

        // update last
        else if (r.Date == lastResult.Date)
        {
            lastResult.Sma = r.Sma;
            SendToChain(lastResult);
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

    // calculate with provider cache
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
