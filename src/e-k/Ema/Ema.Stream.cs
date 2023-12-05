namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

public partial class Ema : ChainObserver<EmaResult>
{
    // constructor
    public Ema(
        ChainProvider provider,
        int lookbackPeriods)
        : base(provider)
    {
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        Initialize();

        // subscribe to provider
        unsubscriber = provider != null
           ? provider.Subscribe(this)
           : throw new ArgumentNullException(nameof(provider));
    }

    // PROPERTIES

    // common

    private int LookbackPeriods { get; set; }
    private double K { get; set; }

    // METHODS

    // handle chain arrival
    public override void OnNext((Act act, DateTime date, double price) value)
    {
        // determine incremental value
        double ema;

        int i = ChainSupplier.Chain.FindIndex(value.date);

        // source unexpectedly not found
        if (i == -1)
        {
            throw new InvalidOperationException("Matching source history not found.");
        }

        // normal
        else if (i >= LookbackPeriods - 1)
        {
            IReusableResult last = Cache[i - 1];  // prior EMA

            // normal
            if (!double.IsNaN(last.Value))
            {
                ema = Increment(K, last.Value, value.price);
            }

            // set first value (normal) or reset
            // when prior EMA was incalculable
            else
            {
                double sum = 0;
                for (int w = i - LookbackPeriods + 1; w <= i; w++)
                {
                    sum += ChainSupplier.Chain[w].Value;
                }

                ema = sum / LookbackPeriods;
            }
        }

        // warmup periods are never calculable
        else
        {
            ema = double.NaN;
        }

        // candidate result
        EmaResult r = new()
        {
            Date = value.date,
            Ema = ema.NaN2Null()
        };

        // save to cache
        Act act = CacheChainorPerAction(value.act, r, ema);

        // send to observers
        NotifyObservers(act, r);

        // TODO: use "update" approach, but maybe with pruning when not matched with supplier?
        // but would need to handle differently here as it triggers on non-AddNew scenarios.

        // rebuild forward values, when needed
        if (act != Act.AddNew)
        {
            ClearCache(r.Date);
            RebuildCache(r.Date);
        }
    }

    // delete cache between index values
    // usually called from inherited ClearCache(fromDate)
    internal override void ClearCache(int fromIndex, int toIndex)
    {
        // delete and deliver instruction,
        // in reverse order to prevent recompositions

        for (int i = toIndex; i >= fromIndex; i--)
        {
            EmaResult r = Cache[i];
            Act act = CacheChainorPerAction(Act.Delete, r, double.NaN);
            NotifyObservers(act, r);
        }
    }
}
