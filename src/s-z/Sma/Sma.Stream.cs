namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAMING)

public class Sma<TIn> : AbstractChainInChainOut<TIn, SmaResult>, ISma
    where TIn : struct, IReusable
{
    public Sma(
        IChainProvider<TIn> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Sma.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;

        // subscribe to chain provider
        Subscription = provider != null
           ? provider.Subscribe(this)
           : throw new ArgumentNullException(nameof(provider));
    }

    public int LookbackPeriods { get; }


    # region METHODS

    public override string ToString()
        => $"SMA({LookbackPeriods})";

    protected override void OnNextArrival(Act act, TIn inbound)
    {
        int i;
        SmaResult r;

        // handle deletes
        if (act == Act.Delete)
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
            double sma;

            if (i >= LookbackPeriods - 1)
            {
                double sum = 0;
                for (int w = i - LookbackPeriods + 1; w <= i; w++)
                {
                    sum += ProviderCache[w].Value;
                }

                sma = sum / LookbackPeriods;
            }

            // warmup periods are never calculable
            else
            {
                sma = double.NaN;
            }

            // candidate result
            r = new(
                Timestamp: inbound.Timestamp,
                Sma: sma.NaN2Null());
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
