namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)
public class EmaObs : QuoteObserver
{
    public EmaObs(
        QuoteProvider? provider,
        int lookbackPeriods)
    {
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        ProtectedResults = new();
        WarmupValue = 0;
        Provider = provider;

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
    private QuoteProvider? Provider { get; set; }

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

    // calculate initial cache of quotes
    public void Initialize(IEnumerable<Quote> quotes)
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        if (quotes != null)
        {
            for (int i = 0; i < quotesList.Count; i++)
            {
                Add(quotesList[i]);
            }
        }
    }

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
        // check for duplicate
        int length = ProtectedResults.Count;
        bool dup = length != 0 && ProtectedResults[length - 1].Date == tp.Date;

        // empty candidate result
        EmaResult r = new(tp.Date);

        // initialization periods
        if (length < LookbackPeriods - 1)
        {
            // add if not duplicate
            if (!dup)
            {
                ProtectedResults.Add(r);
                WarmupValue += tp.Value;
            }

            return r;
        }

        EmaResult last = ProtectedResults[length - 1];

        // initialize with SMA
        if (length == LookbackPeriods - 1)
        {
            WarmupValue += tp.Value;
            r.Ema = (WarmupValue / LookbackPeriods).NaN2Null();
            ProtectedResults.Add(r);
            return r;
        }

        // add new bar
        if (tp.Date > last.Date)
        {
            // calculate incremental value
            double lastEma = (last.Ema == null) ? double.NaN : (double)last.Ema;
            double newEma = Increment(tp.Value, lastEma, K);

            r.Ema = newEma.NaN2Null();
            ProtectedResults.Add(r);
        }

        // update bar
        else if (tp.Date == last.Date)
        {
            // get prior last EMA
            EmaResult prior = ProtectedResults[length - 2];

            double priorEma = (prior.Ema == null) ? double.NaN : (double)prior.Ema;
            last.Ema = Increment(tp.Value, priorEma, K);
        }

        // nuclear reset if old bar enters and provider available
        else if (Provider != null && tp.Date < last.Date)
        {
            r = RestartProvider(Provider, r);
        }

        return r;
    }

    private EmaResult RestartProvider(QuoteProvider provider, EmaResult r)
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
