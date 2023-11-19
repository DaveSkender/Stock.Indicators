namespace Skender.Stock.Indicators;

// USE (STREAMING)
public class Use<TQuote> : QuoteInTupleOut<TQuote>
    where TQuote : IQuote, new()
{
    public Use(
        QuoteProvider<TQuote> provider,
        CandlePart candlePart)
    {
        QuoteSupplier = provider;
        CandlePartSelection = candlePart;
        Initialize();
    }

    // PROPERTIES

    public IEnumerable<(DateTime Date, double Value)> Results => ProtectedTuples;

    private CandlePart CandlePartSelection { get; set; }

    // METHODS

    // handle new arrival
    public override void OnNext((Act act, TQuote quote) value)
    {
        (DateTime d, double v)
            = value.quote.ToTuple(CandlePartSelection);

        CacheAndDeliverTuple((value.act, d, v));
    }

    // initialize and preload existing quote cache
    private void Initialize()
    {
        List<(DateTime, double)> tuples = QuoteSupplier
            .ProtectedQuotes
            .ToTuple(CandlePartSelection);

        for (int i = 0; i < tuples.Count; i++)
        {
            (DateTime date, double value) = tuples[i];
            CacheAndDeliverTuple((Act.AddNew, date, value));
        }

        Subscribe();
    }
}
