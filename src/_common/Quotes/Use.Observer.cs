namespace Skender.Stock.Indicators;

// USE (STREAMING)
public class UseObserver : TupleProvider
{
    public UseObserver(
        QuoteProvider? provider,
        CandlePart candlePart)
    {
        Supplier = provider;
        CandlePartSelection = candlePart;
        Initialize();
    }

    // PROPERTIES

    public IEnumerable<(DateTime Date, double Value)> Results => ProtectedTuples;

    private CandlePart CandlePartSelection { get; set; }

    // NON-STATIC METHODS

    // handle quote arrival
    public override void OnNext(Quote value) => HandleArrival(value);

    // add new quote
    internal void HandleArrival(Quote quote)
    {
        // candidate result
        (DateTime date, double value) r = quote.ToTuple(CandlePartSelection);

        // initialize
        int length = ProtectedTuples.Count;

        if (length == 0)
        {
            AddSend(r);
            return;
        }

        // check against last entry
        (DateTime lastDate, _) = ProtectedTuples[length - 1];

        // add new
        if (r.date > lastDate)
        {
            AddSend(r);
        }

        // update last
        else if (r.date == lastDate)
        {
            ProtectedTuples[length - 1] = r;
        }

        // late arrival
        else
        {
            AddSend(r);
            throw new NotImplementedException();
        }
    }

    // calculate initial cache of quotes
    private void Initialize()
    {
        if (Supplier != null)
        {
            ProtectedTuples = Supplier
                .ProtectedQuotes
                .ToTuple(CandlePartSelection);
        }

        Subscribe();
    }
}
