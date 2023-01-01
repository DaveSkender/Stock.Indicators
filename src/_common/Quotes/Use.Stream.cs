namespace Skender.Stock.Indicators;

// USE (STREAMING)
public class UseObserver : QuoteObserverTupleProvider
{
    public UseObserver(
        QuoteProvider? provider,
        CandlePart candlePart)
    {
        Provider = provider;

        CandlePartSelection = candlePart;

        if (provider != null)
        {
            Initialize(provider.GetQuotesList());
            Subscribe(provider);
        }
    }

    // PROPERTIES

    public IEnumerable<(DateTime Date, double Value)> Results => ProtectedOutput;

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
    internal void Add(Quote quote)
    {
        // candidate result
        (DateTime date, double value) tp = quote.ToTuple(CandlePartSelection);

        // initialize
        int length = ProtectedOutput.Count;

        if (length == 0)
        {
            Add(tp);
            return;
        }

        // check against last entry
        (DateTime lastDate, _) = ProtectedOutput[length - 1];

        // add bar
        if (tp.date > lastDate)
        {
            Add(tp);
        }

        // update bar
        else if (tp.date == lastDate)
        {
            ProtectedOutput[length - 1] = tp;
        }

        // old bar
        else if (Provider != null && tp.date < lastDate)
        {
            Reset(Provider);
        }
    }

    // calculate initial cache of quotes
    internal void Initialize(IEnumerable<Quote> quotes)
    {
        if (quotes != null)
        {
            ProtectedOutput = quotes.ToTuple(CandlePartSelection);
        }
    }

    private void Reset(QuoteProvider provider)
    {
        Unsubscribe();
        Initialize(provider.GetQuotesList());
        Subscribe(provider);
    }
}
