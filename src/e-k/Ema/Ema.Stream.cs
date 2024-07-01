namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

public class Ema<TIn>
    : AbstractChainInChainOut<TIn, EmaResult>, IEma
    where TIn : struct, IReusable
{
    public Ema(
        IChainProvider<TIn> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Ema.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        // subscribe to provider
        Subscription = provider != null
           ? provider.Subscribe(this)
           : throw new ArgumentNullException(nameof(provider));
    }

    public int LookbackPeriods { get; }
    public double K { get; }


    # region METHODS

    // string label
    public override string ToString()
        => $"EMA({LookbackPeriods})";

    // handle chain arrival
    protected override void OnNextArrival(Act act, TIn inbound)
    {
        int i;
        EmaResult r;

        // handle deletes
        if (act is Act.Delete)
        {
            i = Cache.FindIndex(inbound.Timestamp);

            // cache entry unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching cache entry not found.");
            }

            r = Cache[i];
        }

        // calculate incremental value
        else
        {
            i = Provider.FindIndex(inbound.Timestamp);

            // source unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching source history not found.");
            }

            // normal
            double ema;

            if (i >= LookbackPeriods - 1)
            {
                IReusable last = Cache[i - 1];  // prior EMA

                // normal
                if (!double.IsNaN(last.Value))
                {
                    ema = Ema.Increment(K, last.Value, inbound.Value);
                }

                // set first value (normal) or reset
                // when prior EMA was incalculable
                else
                {
                    double sum = 0;
                    for (int w = i - LookbackPeriods + 1; w <= i; w++)
                    {
                        sum += ProviderCache[w].Value;
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
            r = new(
                Timestamp: inbound.Timestamp,
                Ema: ema.NaN2Null());
        }

        // save to cache
        act = ModifyCache(act, r);

        // send to observers
        NotifyObservers(act, r);

        // cascade update forward values (recursively)
        if (act != Act.AddNew && i < ProviderCache.Count - 1)
        {
            int next = act == Act.Delete ? i : i + 1;
            TIn value = ProviderCache[next];
            OnNextArrival(Act.Update, value);
        }
    }
    #endregion
}
