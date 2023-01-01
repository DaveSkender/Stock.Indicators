namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)
public class EmaObserver : QuoteObserver
{
    public EmaObserver(
        QuoteProvider? provider,
        int lookbackPeriods)
    {
        Provider = provider;
        ProtectedResults = new();

        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        WarmupValue = 0;

        if (provider != null)
        {
            Initialize(provider.GetQuotesList());
            Subscribe(provider);
        }
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
    public override void OnNext(Quote value)
    {
        if (value != null)
        {
            Add(value);
        }
        else
        {
            throw new InvalidQuotesException(nameof(value), "Quote cannot be null.");
        }
    }

    // add new whole quote
    internal EmaResult Add(
        Quote quote,
        CandlePart candlePart = CandlePart.Close)
    {
        (DateTime Date, double Value) tuple = quote.ToTuple(candlePart);
        return Add(tuple);
    }

    // add new tuple quote
    internal EmaResult Add(
        (DateTime Date, double Value) tp)
    {
        // candidate result (empty)
        EmaResult r = new(tp.Date);

        // initialize
        int length = ProtectedResults.Count;

        if (length == 0)
        {
            ProtectedResults.Add(r);
            WarmupValue += tp.Value;
            return r;
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

            return r;
        }

        // initialize with SMA
        if (length == LookbackPeriods - 1)
        {
            WarmupValue += tp.Value;
            r.Ema = (WarmupValue / LookbackPeriods).NaN2Null();
            ProtectedResults.Add(r);
            return r;
        }

        // add bar
        if (tp.Date > last.Date)
        {
            // calculate incremental value
            double lastEma = (last.Ema == null) ? double.NaN : (double)last.Ema;
            double newEma = Increment(tp.Value, lastEma, K);

            r.Ema = newEma.NaN2Null();
            ProtectedResults.Add(r);
            return r;
        }

        // update bar
        else if (tp.Date == last.Date)
        {
            // get prior last EMA
            EmaResult prior = ProtectedResults[length - 2];

            double priorEma = (prior.Ema == null) ? double.NaN : (double)prior.Ema;
            last.Ema = Increment(tp.Value, priorEma, K);
            return last;
        }

        // old bar
        else if (Provider != null && tp.Date < last.Date)
        {
            return Reset(Provider, r);
        }

        return r;
    }

    // calculate initial cache of quotes
    internal void Initialize(IEnumerable<Quote> quotes)
    {
        if (quotes != null)
        {
            List<Quote> quotesList = quotes
                .ToSortedList();

            for (int i = 0; i < quotesList.Count; i++)
            {
                Add(quotesList[i]);
            }
        }
    }

    private EmaResult Reset(QuoteProvider provider, EmaResult r)
    {
        Unsubscribe();

        ProtectedResults = new();
        WarmupValue = 0;

        Initialize(provider.GetQuotesList());
        Subscribe(provider);

        int length = ProtectedResults.Count;

        return length > 0 ? ProtectedResults[length - 1] : r;
    }
}
