namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)
public class EmaObs : QuoteObserver
{
    public EmaObs(
        QuoteProvider provider,
        int lookbackPeriods)
    {
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        ProtectedResults = new();
        QuotesCache = new(lookbackPeriods);

        if (provider != null)
        {
            Initialize(provider.GetQuotesList());
            Subscribe(provider);
        }
    }

    // PROPERTIES
    public Guid UUID { get; }
    public IEnumerable<EmaResult> Results => ProtectedResults;

    internal List<EmaResult> ProtectedResults { get; set; }
    private List<(DateTime Date, double Value)> QuotesCache { get; set; }

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
        if (quote == null)
        {
            throw new InvalidQuotesException(nameof(quote), quote, "No quote provided.");
        }

        (DateTime Date, double Value) tuple = quote.ToTuple(candlePart);
        return Add(tuple);
    }

    // add new tuple quote
    internal EmaResult Add(
        (DateTime Date, double Value) tpQuote)
    {
        // empty candidate result
        EmaResult r = new(tpQuote.Date);

        // maintain quote cache
        int initCacheSize = QuotesCache.Count;
        bool isDupQuote = initCacheSize != 0 && QuotesCache[initCacheSize - 1].Date == tpQuote.Date;

        // handle same-date (remove last)
        if (initCacheSize > 0 && isDupQuote)
        {
            QuotesCache[initCacheSize - 1] = tpQuote;
        }
        else
        {
            QuotesCache.Add(tpQuote);
        }

        // get next position
        int i = ProtectedResults.Count;

        // handle initialization periods
        if (i < LookbackPeriods - 1)
        {
            // add if not duplicate
            if (!isDupQuote)
            {
                r = new(tpQuote.Date) { Ema = null };
                ProtectedResults.Add(r);
            }

            return r;
        }

        EmaResult last = ProtectedResults[i - 1];

        // [re-]initialize with SMA
        if (i == LookbackPeriods - 1 // initial
        || (i > LookbackPeriods && last.Ema == null)) // reset
        {
            double sumValue = 0;
            int cacheSize = QuotesCache.Count;
            for (int p = cacheSize - 1; p >= cacheSize - LookbackPeriods; p--)
            {
                (DateTime _, double value) = QuotesCache[p];
                sumValue += value;
            }

            r = new(tpQuote.Date) { Ema = (sumValue / LookbackPeriods).NaN2Null() };
            ProtectedResults.Add(r);
            return r;
        }

        // add new bar
        if (tpQuote.Date > last.Date)
        {
            // calculate incremental value
            double lastEma = (last.Ema == null) ? double.NaN : (double)last.Ema;
            double newEma = Increment(tpQuote.Value, lastEma, K);

            r = new(tpQuote.Date) { Ema = newEma.NaN2Null() };
            ProtectedResults.Add(r);
        }

        // update bar
        else if (tpQuote.Date == last.Date)
        {
            // get prior last EMA
            EmaResult prior = ProtectedResults[i - 2];

            double priorEma = (prior.Ema == null) ? double.NaN : (double)prior.Ema;
            last.Ema = Increment(tpQuote.Value, priorEma, K);
        }

        return r;
    }
}
