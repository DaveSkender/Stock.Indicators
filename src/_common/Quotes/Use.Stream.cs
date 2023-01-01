namespace Skender.Stock.Indicators;

// USE (STREAMING)
public class UseObserver : TupleProvider
{
    public UseObserver(
        QuoteProvider? provider,
        CandlePart candlePart)
    {
        Provider = provider;

        CandlePartSelection = candlePart;

        Initialize();
    }

    // PROPERTIES

    public IEnumerable<(DateTime Date, double Value)> Results => ProtectedTuples;

    private CandlePart CandlePartSelection { get; set; }

    // NON-STATIC METHODS

    // handle quote arrival
    public override void OnNext(Quote value)
    {
        if (value == null)
        {
            throw new InvalidQuotesException(nameof(value), "Quote cannot be null.");
        }

        Add(value);
    }

    // add new quote
    internal void Add(Quote quote)
    {
        // candidate result
        (DateTime date, double value) tp = quote.ToTuple(CandlePartSelection);

        // initialize
        int length = ProtectedTuples.Count;

        if (length == 0)
        {
            Add(tp);
            return;
        }

        // check against last entry
        (DateTime lastDate, _) = ProtectedTuples[length - 1];

        // add bar
        if (tp.date > lastDate)
        {
            Add(tp);
        }

        // update bar
        else if (tp.date == lastDate)
        {
            ProtectedTuples[length - 1] = tp;
        }

        // old bar
        else if (Provider != null && tp.date < lastDate)
        {
            Add(tp);
            Reset();
        }
    }

    // calculate initial cache of quotes
    private void Initialize()
    {
        if (Provider != null)
        {
            ProtectedTuples = Provider
                .ProtectedQuotes
                .ToTuple(CandlePartSelection);
        }

        Subscribe();
    }

    // recalculate cache
    private void Reset()
    {
        Unsubscribe();
        ProtectedTuples = new();
        Initialize();
    }
}
