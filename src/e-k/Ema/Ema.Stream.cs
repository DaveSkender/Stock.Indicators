using System.ComponentModel;

namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)
public class EmaObserver : TupleObserver
{
    public EmaObserver(
        TupleProvider? provider,
        int lookbackPeriods)
    {
        Provider = provider;
        ProtectedResults = new();

        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        WarmupValue = 0;

        Initialize();
    }

    // PROPERTIES

    public IEnumerable<EmaResult> Results => ProtectedResults;
    internal List<EmaResult> ProtectedResults { get; set; }

    private double WarmupValue { get; set; }
    private int LookbackPeriods { get; set; }
    private double K { get; set; }

    // STATIC METHODS

    // parameter validation
    internal static void Validate(
    int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for EMA.");
        }
    }

    // incremental calculation
    internal static double Increment(double newValue, double lastEma, double k)
        => lastEma + (k * (newValue - lastEma));

    // NON-STATIC METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Add(value);

    // add new tuple quote
    internal void Add((DateTime Date, double Value) tuple)
    {
        // candidate result (empty)
        EmaResult r = new(tuple.Date);

        // initialize
        int length = ProtectedResults.Count;

        if (length == 0)
        {
            ProtectedResults.Add(r);
            WarmupValue += tuple.Value;
            return;
        }

        // check against last entry
        EmaResult last = ProtectedResults[length - 1];

        // initialization periods
        if (length < LookbackPeriods - 1)
        {
            // add if not duplicate
            if (last.Date != r.Date)
            {
                ProtectedResults.Add(r);
                WarmupValue += tuple.Value;
            }

            return;
        }

        // initialize with SMA
        if (length == LookbackPeriods - 1)
        {
            WarmupValue += tuple.Value;
            r.Ema = (WarmupValue / LookbackPeriods).NaN2Null();
            ProtectedResults.Add(r);
            return;
        }

        // add bar
        if (tuple.Date > last.Date)
        {
            // calculate incremental value
            double lastEma = (last.Ema == null) ? double.NaN : (double)last.Ema;
            double newEma = Increment(tuple.Value, lastEma, K);

            r.Ema = newEma.NaN2Null();
            ProtectedResults.Add(r);
            return;
        }

        // update bar
        else if (tuple.Date == last.Date)
        {
            // get prior last EMA
            EmaResult prior = ProtectedResults[length - 2];

            double priorEma = (prior.Ema == null) ? double.NaN : (double)prior.Ema;
            last.Ema = Increment(tuple.Value, priorEma, K);
            return;
        }

        // old bar
        else if (Provider != null && tuple.Date < last.Date)
        {
            Reset();
        }
    }

    // calculate with provider cache
    private void Initialize()
    {
        if (Provider != null)
        {
            List<(DateTime, double)> tuples = Provider
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
        WarmupValue = 0;
        Initialize();
    }
}
