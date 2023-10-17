namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

/// <summary>See the <see href = "https://dotnet.stockindicators.dev/indicators/Ema/">
///  Stock Indicators for .NET online guide</see> for more information.</summary>
public partial class Ema : ChainProvider
{
    // constructor
    public Ema(
        TupleProvider provider,
        int lookbackPeriods)
    {
        Supplier = provider;
        ProtectedResults = new();

        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

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

    // increment calculation
    internal static double Increment(double k, double lastEma, double newValue)
        => lastEma + (k * (newValue - lastEma));

    // NON-STATIC METHODS

    // handle quote arrival
    public override void OnNext((DateTime Date, double Value) value) => Add(value);

    // add new tuple quote
    internal void Add((DateTime Date, double Value) tp)
    {
        // candidate result (empty)
        EmaResult r = new(tp.Date);

        // initialize
        int length = ProtectedResults.Count;

        if (length == 0)
        {
            ProtectedResults.Add(r);
            WarmupValue += tp.Value;
            SendToChain(r);
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
                WarmupValue += tp.Value;
            }

            return;
        }

        // initialize with SMA
        if (length == LookbackPeriods - 1)
        {
            WarmupValue += tp.Value;
            r.Ema = (WarmupValue / LookbackPeriods).NaN2Null();
            ProtectedResults.Add(r);
            SendToChain(r);
            return;
        }

        // add new
        if (r.Date > last.Date)
        {
            // calculate incremental value
            double lastEma = (last.Ema == null) ? double.NaN : (double)last.Ema;
            double newEma = Increment(K, lastEma, tp.Value);

            r.Ema = newEma.NaN2Null();
            ProtectedResults.Add(r);
            SendToChain(r);
            return;
        }

        // update last
        else if (r.Date == last.Date)
        {
            // get prior last EMA
            EmaResult prior = ProtectedResults[length - 2];

            double priorEma = (prior.Ema == null) ? double.NaN : (double)prior.Ema;
            last.Ema = Increment(K, priorEma, tp.Value);
            SendToChain(last);
            return;
        }

        // late arrival
        else
        {
            // heal
            throw new NotImplementedException();
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
