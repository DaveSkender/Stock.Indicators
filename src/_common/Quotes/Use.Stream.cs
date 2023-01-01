namespace Skender.Stock.Indicators;

// USE (STREAMING)
public class UseObserver : QuoteObserver
{
    public UseObserver(
        QuoteProvider? provider,
        CandlePart candlePart)
    {
        Provider = provider;
        ProtectedResults = new();

        CandlePartSelection = candlePart;

        if (provider != null)
        {
            Initialize(provider.GetQuotesList());
            Subscribe(provider);
        }
    }

    // PROPERTIES

    public IEnumerable<(DateTime Date, double Value)> Results => ProtectedResults;
    internal List<(DateTime Date, double Value)> ProtectedResults { get; set; }

    private CandlePart CandlePartSelection { get; set; }

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

    // add new quote
    internal (DateTime, double) Add(Quote quote)
    {
        // candidate result
        (DateTime date, double value) tp = quote.ToTuple(CandlePartSelection);

        // initialize
        int length = ProtectedResults.Count;

        if (length == 0)
        {
            ProtectedResults.Add(tp);
            return tp;
        }

        // check against last entry
        (DateTime lastDate, _) = ProtectedResults[length - 1];

        // add bar
        if (tp.date > lastDate)
        {
            ProtectedResults.Add(tp);
        }

        // update bar
        else if (tp.date == lastDate)
        {
            ProtectedResults[length - 1] = tp;
        }

        // old bar
        else if (Provider != null && tp.date < lastDate)
        {
            tp = Reset(Provider, tp);
        }

        return tp;
    }

    // calculate initial cache of quotes
    internal void Initialize(IEnumerable<Quote> quotes)
    {
        if (quotes != null)
        {
            ProtectedResults = quotes.ToTuple(CandlePartSelection);
        }
    }

    private (DateTime, double) Reset(QuoteProvider provider, (DateTime, double) r)
    {
        Unsubscribe();
        Initialize(provider.GetQuotesList());
        Subscribe(provider);

        int length = ProtectedResults.Count;

        return length > 0 ? ProtectedResults[length - 1] : r;
    }
}
