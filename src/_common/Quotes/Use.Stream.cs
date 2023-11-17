namespace Skender.Stock.Indicators;

// USE (STREAMING)
public class Use : ObsQuoteSendTuple
{
    public Use(
        QuoteProvider provider,
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

    // handle quote arrival
    public override void OnNext(Quote value)
    {
        if (value is not null)
        {
            AddToTupleProvider(value.ToTuple(CandlePartSelection));
        }
    }

    // calculate initial cache of quotes
    private void Initialize()
    {
        List<(DateTime, double)> tuples = QuoteSupplier
            .ProtectedQuotes
            .ToTuple(CandlePartSelection);

        AddToTupleProvider(tuples);

        Subscribe();
    }
}
