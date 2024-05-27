namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

public partial class Ema : ChainObserver<EmaResult>, IEma
{
    // constructor
    public Ema(
        ChainProvider provider,
        int lookbackPeriods)
        : base(provider, isChainor: true)
    {
        Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        RebuildCache();

        // subscribe to provider
        unsubscriber = provider != null
           ? provider.Subscribe(this)
           : throw new ArgumentNullException(nameof(provider));
    }

    // PROPERTIES

    public int LookbackPeriods { get; private set; }
    public double K { get; private set; }

    // METHODS

    // handle chain arrival
    public override void OnNext((Act act, DateTime date, double price) value)
    {
        int i;
        double ema;

        List<(DateTime _, double value)> supplier = ChainSupplier.Chain;

        // handle deletes
        if (value.act == Act.Delete)
        {
            i = Cache.FindIndex(value.date);
            ema = Cache[i].Ema.Null2NaN();
        }

        // handle new values
        else
        {
            i = supplier.FindIndex(value.date);

            // source unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching source history not found on arrival.");
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
                        sum += supplier[w].value;
                    }

                    ema = sum / LookbackPeriods;
                }
            }

            // warmup periods are never calculable
            else
            {
                ema = double.NaN;
            }
        }

        // candidate result
        EmaResult r = new() {
            Timestamp = value.date,
            Ema = ema.NaN2Null()
        };

        // save to cache
        Act act = CacheChainorPerAction(value.act, r, ema);

        // send to observers
        NotifyObservers(act, r);

        // update forward values
        if (act != Act.AddNew && i < supplier.Count - 1)
        {
            // cascade updates gracefully
            int next = act == Act.Delete ? i : i + 1;
            (DateTime d, double v) = supplier[next];
            OnNext((Act.Update, d, v));
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
