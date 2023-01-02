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

    // incremental calculation
    internal static double Increment(
        List<(DateTime Date, double Value)> tpList,
        int index,
        int lookbackPeriods)
    {
        if (index >= lookbackPeriods - 1)
        {
            double sumSma = 0;
            for (int p = index - lookbackPeriods + 1; p <= index; p++)
            {
                (DateTime _, double pValue) = tpList[p];
                sumSma += pValue;
            }

            return sumSma / lookbackPeriods;
        }
        else
        {
            return double.NaN;
        }
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

        // find result in re-composed results
        int sourceIndex = Supplier.ProtectedTuples
            .FindIndex(x => x.Date == tp.Date);

        // candidate result
        SmaResult r = new(tp.Date)
        {
            Sma = Increment(
                Supplier.ProtectedTuples,
                sourceIndex,
                LookbackPeriods)
            .NaN2Null()
        };

        // initialize
        int length = ProtectedResults.Count;

        if (length == 0)
        {
            ProtectedResults.Add(r);
            SendToChain(r);
            return;
        }

        // check against last entry
        SmaResult last = ProtectedResults[length - 1];

        // add bar
        if (tp.Date > last.Date)
        {
            ProtectedResults.Add(r);
            SendToChain(r);
            return;
        }

        // update bar
        else if (tp.Date == last.Date)
        {
            last.Sma = r.Sma;
            SendToChain(last);
            return;
        }

        // old bar
        else if (tp.Date < last.Date)
        {
            Reset();

            // find result in re-composed results
            int foundIndex = ProtectedResults
                .FindIndex(x => x.Date == r.Date);

            SendToChain(ProtectedResults[foundIndex]);
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

    // recalculate cache
    private void Reset()
    {
        Unsubscribe();
        ProtectedResults = new();
        Initialize();
    }
}
